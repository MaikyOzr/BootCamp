using BootCamp.RabitMqPublisher;
using BootCamp.UserService.Application;
using BootCamp.UserService.Application.Auth;
using BootCamp.UserService.Application.Auth.Models.Request;
using BootCamp.UserService.Application.Validation;
using BootCamp.UserService.Database;
using BootCamp.UserService.Options;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using StackExchange.Redis;

namespace BootCamp.UserService;

public static class DependencyInjection
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddUserService(configuration);
        services.AddRabbitMq(configuration);
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddOpenApi();
        services.AddLogging();
        services.AddHttpClient();
        services.AddHttpClient();
        services.AddValidation();
        services.AddCommand();
        services.AddProblemDetails(opt =>
        {
            opt.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        services.AddMemoryCache();
        services.AddDbContext<UserDbContext>((sp, db) =>
        {
            var cs = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value.DefaultConnection;

            db.UseNpgsql(cs);
        });

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));
    }
    private static void AddUserService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ConnectionStringsOptions>()
            .Bind(configuration.GetSection(ConnectionStringsOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(x => !string.IsNullOrWhiteSpace(x.DefaultConnection),
            "ConnectionStrings:DefaultConnection must be set")
            .ValidateOnStart();
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(x => !string.IsNullOrWhiteSpace(x.SecretKey), "Jwt:SecretKey must be set")
            .ValidateOnStart();
        services.AddScoped<JwtTokenService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        services.AddAuthorization();
    }

    private static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RabbitMqOptions>()
            .Bind(configuration.GetSection(RabbitMqOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.UserName), "UserName is required")
            .Validate(opt => !string.IsNullOrWhiteSpace(opt.Password), "Password is required")
            .ValidateOnStart();

        services.AddSingleton<IConnection>(sp =>
        {
            var opt = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
            var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("RabbitMQ");

            logger.LogInformation("🐇 Connecting to RabbitMQ → {Host}:{Port}, vhost={VHost}, user={User}",
                opt.HostName, opt.Port, opt.VirtualHost, opt.UserName);

            var factory = new ConnectionFactory
            {
                HostName = opt.HostName,
                Port = opt.Port,
                UserName = opt.UserName,
                Password = opt.Password,
                VirtualHost = opt.VirtualHost,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
            };

            var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();

            logger.LogInformation("✅ RabbitMQ connected successfully");
            return connection;
        });

        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

        return services;
    }
    private static IServiceCollection AddValidation(this IServiceCollection services) 
    {
        services.AddScoped<IValidator<SignInRequst>, SignInRequestValidation>();
        services.AddScoped<IValidator<SignUpRequest>, SignUpRqeustValidation>();
        return services;
    }

    private static IServiceCollection AddCommand(this IServiceCollection services) 
    {
        services.AddScoped<SignInCommand>();
        services.AddScoped<SignUpCommand>();
        return services;
    }

}
