using Chima.Api.Filters;
using Chima.Application.Interfaces;
using Chima.Application.Services;
using Chima.Infrastructure.Configuration;
using Chima.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace Chima.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChimaPersistence(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("Default")
                 ?? throw new InvalidOperationException("Connection string 'Default' missing.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(cs, npg =>
            {
                npg.UseVector();
                npg.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            }));

        return services;
    }

    public static IServiceCollection AddChimaAuth(this IServiceCollection services, IConfiguration config)
    {
        var jwt = config.GetSection("Jwt");
        var secret = jwt["Secret"] ?? throw new InvalidOperationException("Jwt:Secret missing.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();
        services.AddChimaAuthInfrastructure(config);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ConversationService>();
        services.AddScoped<MessageService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<QuotaEnforcementFilter>();

        services.AddValidatorsFromAssembly(typeof(AuthService).Assembly);

        services.AddControllers(opts =>
        {
            opts.Filters.AddService<QuotaEnforcementFilter>();
        });

        return services;
    }

    public static IServiceCollection AddChimaSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chima API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT access token."
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    public static IServiceCollection AddChimaCors(this IServiceCollection services, IConfiguration config)
    {
        var origins = (config["Cors:AllowedOrigins"] ?? "http://localhost:3000,http://localhost:3001")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        services.AddCors(o => o.AddPolicy("Default", p =>
            p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

        return services;
    }
}