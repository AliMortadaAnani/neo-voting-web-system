using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NeoVoting.Application.Exceptions;
using NeoVoting.Application.Services;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Application.Validators;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.ErrorHandling;
using NeoVoting.Domain.IdentityEntities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Infrastructure.DbContext;
using NeoVoting.Infrastructure.Repositories;
using System.Reflection;
using System.Text;

namespace NeoVoting.API.StartupExtensions
{
    public static class ConfigureServicesExtension
    {
        /// <summary>
        /// Master extension method that calls all segregated configuration groups.
        /// </summary>
        public static WebApplicationBuilder ConfigureAllServices(this WebApplicationBuilder builder)
        {
            builder.ConfigureDatabase()
                   .ConfigureIdentity()
                  // .ConfigureJwtAuthentication()
                   .ConfigureApiAndControllers()
                   .ConfigureExceptions()
                   .ConfigureSwagger()
                   .ConfigureHttpClients()
                 //.ConfigureCors()
                   .ConfigureRepositoriesAndServices()
                   .ConfigureValidation();

            return builder;
        }

        public static WebApplicationBuilder ConfigureDatabase(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            return builder;
        }

        public static WebApplicationBuilder ConfigureIdentity(this WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3;
                options.Password.RequiredUniqueChars = 1;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            return builder;
        }

        public static WebApplicationBuilder ConfigureJwtAuthentication(this WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/problem+json";

                        var problem = new ProblemDetails
                        {
                            Type = nameof(ProblemDetails401ErrorTypes.Auth_InvalidToken),
                            Title = "Unauthorized",
                            Status = StatusCodes.Status401Unauthorized,
                            Detail = "Authentication failed. Token is missing, invalid, or expired.",
                            Instance = context.Request.Path
                        };

                        await context.Response.WriteAsJsonAsync(problem);
                    },

                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/problem+json";

                        var problem = new ProblemDetails
                        {
                            Type = nameof(ProblemDetails403ErrorTypes.Auth_ForbiddenAccess),
                            Title = "Forbidden",
                            Status = StatusCodes.Status403Forbidden,
                            Detail = "You do not have permission to access this resource.",
                            Instance = context.Request.Path
                        };

                        await context.Response.WriteAsJsonAsync(problem);
                    }
                };
            });

            return builder;
        }

        public static WebApplicationBuilder ConfigureApiAndControllers(this WebApplicationBuilder builder)
        {
            //builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
           
            builder.Services.AddProblemDetails();

            return builder;
        }

        public static WebApplicationBuilder ConfigureExceptions(this WebApplicationBuilder builder)
        {
            // Register your Custom Handler
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            return builder;
        }

        public static WebApplicationBuilder ConfigureSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "NeoVoting API",
                    Version = "v1",
                    Description = "API for the NeoVoting System"
                });

                //options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                //{
                //    Name = "Authorization",
                //    Description = "Enter the JWT token directly. \r\n\r\nExample: `eyJhbGciOiJIUzI1Ni...`",
                //    In = ParameterLocation.Header,
                //    Type = SecuritySchemeType.Http,
                //    Scheme = "Bearer",
                //    BearerFormat = "JWT"
                //});

                //options.AddSecurityRequirement(new OpenApiSecurityRequirement
                //{
                //    {
                //        new OpenApiSecurityScheme
                //        {
                //            Reference = new OpenApiReference
                //            {
                //                Type = ReferenceType.SecurityScheme,
                //                Id = "Bearer"
                //            }
                //        },
                //        Array.Empty<string>()
                //    }
                //});
            });

            return builder;
        }

        public static WebApplicationBuilder ConfigureHttpClients(this WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;

            builder.Services.AddHttpClient<IGovernmentSystemGateway, GovernmentSystemGateway>(client =>
            {
                var baseUrl = configuration["GovernmentSystem:BaseUrl"]
                              ?? throw new Exception("GovernmentSystem:BaseUrl is missing");

                client.BaseAddress = new Uri(baseUrl);

                var apiKey = configuration["GovernmentSystem:ApiKey"]
                             ?? throw new Exception("GovernmentSystem:ApiKey is missing");

                client.DefaultRequestHeaders.Add("X-Gov-Api-Key", apiKey);
            });

            return builder;
        }

        public static WebApplicationBuilder ConfigureRepositoriesAndServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories
            builder.Services.AddScoped<ICandidateProfileRepository, CandidateProfileRepository>();
            builder.Services.AddScoped<IElectionRepository, ElectionRepository>();
            builder.Services.AddScoped<IElectionStatusRepository, ElectionStatusRepository>();
            builder.Services.AddScoped<IElectionWinnerRepository, ElectionWinnerRepository>();
            builder.Services.AddScoped<IGovernorateRepository, GovernorateRepository>();
            builder.Services.AddScoped<IPublicVoteLogRepository, PublicVoteLogRepository>();
            builder.Services.AddScoped<ISystemAuditLogRepository, SystemAuditLogRepository>();
            builder.Services.AddScoped<IVoteChoiceRepository, VoteChoiceRepository>();
            builder.Services.AddScoped<IVoteRepository, VoteRepository>();
            builder.Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            builder.Services.AddScoped<IElectionStatisticsRepository, ElectionStatisticsRepository>();

            // Services
            builder.Services.AddScoped<ICurrentUserServices, CurrentUserServices>();
            builder.Services.AddScoped<IAuthServices, AuthServices>();
            builder.Services.AddScoped<ITokenServices, TokenServices>();
            builder.Services.AddScoped<IAdminServices, AdminServices>();
            builder.Services.AddScoped<ICandidateServices, CandidateServices>();
            builder.Services.AddScoped<IVoterServices, VoterServices>();
            builder.Services.AddScoped<IGeneralServices, GeneralServices>();
            builder.Services.AddScoped<IFileService, LocalFileService>();

            return builder;
        }

        public static WebApplicationBuilder ConfigureValidation(this WebApplicationBuilder builder)
        {
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<Authentication_ResponseDTO_Validator>();
            builder.Services.AddFluentValidationRulesToSwagger();

            return builder;
        }
    }
}