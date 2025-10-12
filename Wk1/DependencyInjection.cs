using BootCamp.Application.AuthService;
using BootCamp.Application.Feature.Auth.Models.Request;
using BootCamp.Application.Feature.Auth.SingIn.Command;
using BootCamp.Application.Feature.Auth.SingUp.Command;
using BootCamp.Application.Feature.Task.Command;
using BootCamp.Application.Feature.Task.Models.Request;
using BootCamp.Application.Feature.Task.Query;
using BootCamp.Application.Feature.TaskCommentFeature.Command;
using BootCamp.Application.Feature.TaskCommentFeature.Models.Request;
using BootCamp.Application.Feature.TaskCommentFeature.Query;
using BootCamp.Application.Services.ValidationService;
using BootCamp.Domain;
using BootCamp.Infrastruture;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
}
