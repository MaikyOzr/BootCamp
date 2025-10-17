using BootCamp.RabitMqPublisher;
using BootCamp.TaskService.Application.TaskCommentFeature.Command;
using BootCamp.TaskService.Application.TaskCommentFeature.Models.Request;
using BootCamp.TaskService.Application.TaskCommentFeature.Query;
using BootCamp.TaskService.Application.TaskFeature.Command;
using BootCamp.TaskService.Application.TaskFeature.Models.Request;
using BootCamp.TaskService.Application.TaskFeature.Query;
using BootCamp.TaskService.Application.Validation;
using BootCamp.TaskService.Infrastructure;
using BootCamp.TaskService.Web.Exceptions;
using BootCamp.TaskService.Web.Options;
using FluentValidation;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;

namespace BootCamp.TaskService.Web;

public static class DependencyInjection
{
    public static void AddTaskServices(this IServiceCollection services, IConfiguration configuration) 
    {
        services.AddEndpointsApiExplorer();
        services.AddHttpContextAccessor();
        services.AddOpenApi();
        services.AddValidation();
        services.AddCommand();
        services.AddQuery();
        services.AddMemoryCache();
        services.AddRabbitMq(configuration);
        services.AddLogging();
        services.AddHttpClient();
        services.AddProblemDetails(opt => {
            opt.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        services.AddDbContext<TaskServiceDbContext>((sp, db) =>
        {
            var cs = sp.GetRequiredService<IOptions<ConnectionStringsOptions>>().Value.DefaultConnection;

            db.UseNpgsql(cs);
        });
        services.AddExceptionHandler<ValidatorExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));
        services.AddOptions<ConnectionStringsOptions>()
            .Bind(configuration.GetSection(ConnectionStringsOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(x => !string.IsNullOrWhiteSpace(x.DefaultConnection),
            "ConnectionStrings:DefaultConnection must be set")
            .ValidateOnStart();

        services.AddApiVersioning(opt =>
        {
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = new ApiVersion(1);
            opt.ReportApiVersions = true;
        });

        
    }

    private static IServiceCollection AddCommand(this IServiceCollection services)
    {
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

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<UpdateTaskRequest>, UpdateTaskRequestValidation>();
        services.AddScoped<IValidator<UpdateTaskCommentRequest>, UpdateTaskCommentValidation>();
        services.AddScoped<IValidator<CreateTaskCommentRequest>, CreateTaskCommentRequestValidation>();
        services.AddScoped<IValidator<CreateTaskRequest>, CreateTaskRequestValidation>();
        services.AddScoped<IValidator<CreateTaskWithFirstCommentRequest>, CreateTaskWithFirstCommentValidator>();
        return services;
    }
}
