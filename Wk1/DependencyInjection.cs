using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using StackExchange.Redis;
using Wk1.Exceptions;
using Wk1.Options;

namespace Wk1;

public static class DependencyInjection
{
    public static void AddWk1Services(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddOpenApi();
        services.AddValidation();
        services.AddCommand();
        services.AddLogging();
        services.AddHttpClient();
        services.AddProblemDetails(opt => {
            opt.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        services.AddExceptionHandler<ValidatorExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddQuery();
        services.AddMemoryCache();
        services.AddRabbitMq(configuration);

        services.AddOptions<ConnectionStringsOptions>()
            .Bind(configuration.GetSection(ConnectionStringsOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(x=> !string.IsNullOrWhiteSpace(x.DefaultConnection),
            "ConnectionStrings:DefaultConnection must be set")
            .ValidateOnStart();

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(x => !string.IsNullOrWhiteSpace(x.SecretKey), "Jwt:SecretKey must be set")
            .ValidateOnStart();
        services.AddScoped<JwtTokenService>();

        services.AddDbContext<AppDbContext>((sp, db) => 
        {
            var cs = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value.DefaultConnection;

            db.UseNpgsql(cs);
        });

        services.AddIdentityCore<User>(opt => { })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddSignInManager()
        .AddDefaultTokenProviders();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();
        services.AddAuthorization();

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));

        
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<UpdateTaskRequest>, UpdateTaskRequestValidation>();
        services.AddScoped<IValidator<UpdateTaskCommentRequest>, UpdateTaskCommentValidation>();
        services.AddScoped<IValidator<CreateTaskCommentRequest>, CreateTaskCommentRequestValidation>();
        services.AddScoped<IValidator<CreateTaskRequest>, CreateTaskRequestValidation>();
        services.AddScoped<IValidator<SignInRequst>, SignInRequestValidation>();
        services.AddScoped<IValidator<SignUpRequest>, SignUpRqeustValidation>();
        services.AddScoped<IValidator<CreateTaskWithFirstCommentRequest>, CreateTaskWithFirstCommentValidator>();
        return services;
    }

    private static IServiceCollection AddCommand(this IServiceCollection services)
    {
        services.AddScoped<SignInCommand>();
        services.AddScoped<SignUpCommand>();
        services.AddScoped<CreateTaskCommand>();
        services.AddScoped<UpdateTaskCommand>();
        services.AddScoped<CreateTaskCommentCommand>();
        services.AddScoped<UpdateTaskCommentCommand>();
        services.AddScoped<CreateTaskWithFirstCommentCommand>();
        return services;
    }

    private static IServiceCollection AddQuery(this IServiceCollection services)
    {
        services.AddScoped<GetByIdTaskQuery>();
        services.AddScoped<GetAllTaskQuery>();
        services.AddScoped<DeleteTaskQuery>();
        services.AddScoped<GetByIdTaskCommentQuery>();
        services.AddScoped<GetAllTaskCommentQuery>();
        services.AddScoped<DeleteTaskCommentQuery>();
        return services;
    }

    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
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
}
