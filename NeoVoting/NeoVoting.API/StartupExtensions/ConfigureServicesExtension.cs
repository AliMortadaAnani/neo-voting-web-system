using FluentValidation;
using FluentValidation.AspNetCore;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NeoVoting.API.Filters;
using NeoVoting.Application.Exceptions;
using NeoVoting.Application.Services;
using NeoVoting.Application.ServicesContracts;
using NeoVoting.Application.Validators.AdminDTOs;
using NeoVoting.Domain.Entities;
using NeoVoting.Domain.RepositoryContracts;
using NeoVoting.Domain.ResultErrorDomain;
using NeoVoting.Infrastructure.DbContext;
using NeoVoting.Infrastructure.Repositories;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

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
                   .ConfigureJwtAuthenticationAndCookieForRefresh()
                   .ConfigureApiAndControllers()
                   .ConfigureExceptions()
                   .ConfigureSwagger()
                   .ConfigureHttpClients()
                   .ConfigureCors()
                   .ConfigureRepositoriesAndServices()
                   .ConfigureValidation()
                   .ConfigureRateLimiting();

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

        public static WebApplicationBuilder ConfigureJwtAuthenticationAndCookieForRefresh(this WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                // default auth is JWT , but we will use cookie for refresh token storage
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
            })

            .AddCookie(options =>
             {
                 options.Cookie.Name = "RefreshToken";
                 options.Cookie.HttpOnly = true; // prevent JS access
                 options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
                 options.Cookie.SameSite = SameSiteMode.None; //backend and frontend are on different domains
                 options.SlidingExpiration = false; // refresh tokens should not slide
                 options.ExpireTimeSpan = TimeSpan.FromDays(7); // or however long you want
             });

            return builder;
        }

        public static WebApplicationBuilder ConfigureApiAndControllers(this WebApplicationBuilder builder)
        {
            //builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers()
                  .AddJsonOptions(options =>
                  {
                      // Automatically convert ALL enums to their string names in JSON responses
                      options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                  });
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

                // Register the enum filter here
                options.SchemaFilter<EnumSchemaFilter>();

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the JWT token directly. \r\n\r\nExample: `eyJhbGciOiJIUzI1Ni...`",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                       new string[] {}
                    }
                });
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
            builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
            builder.Services.AddScoped<IVoterRepository, VoterRepository>();
            builder.Services.AddScoped<IElectionRepository, ElectionRepository>();
            builder.Services.AddScoped<IElectionStatisticsRepository, ElectionStatisticsRepository>();
            builder.Services.AddScoped<IElectionWinnerRepository, ElectionWinnerRepository>();
            builder.Services.AddScoped<IVoteRepository, VoteRepository>();
            builder.Services.AddScoped<IVoteChoiceRepository, VoteChoiceRepository>();
            builder.Services.AddScoped<IPollRepository, PollRepository>();
            builder.Services.AddScoped<IPollVoteRepository, PollVoteRepository>();
            builder.Services.AddScoped<IPollAnswerRepository, PollAnswerRepository>();
            builder.Services.AddScoped<IEventParticipationRepository, EventParticipationRepository>();
            builder.Services.AddScoped<ISystemAuditLogRepository, SystemAuditLogRepository>();

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

        public static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policyBuilder =>
                {
                    policyBuilder.WithOrigins("http://localhost:3000") // Your Frontend URL
                                 .AllowAnyHeader()
                                 .AllowAnyMethod()
                                 .AllowCredentials();

                    //.WithOrigins("http://localhost:3000") // Your Frontend URL
                });
            });

            return builder;
        }

        public static WebApplicationBuilder ConfigureRateLimiting(this WebApplicationBuilder builder)
        {
            builder.Services.AddRateLimiter(options =>
            {
                // 1. Set the default status code to 429 Too Many Requests
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // 2. Customize the response to match your ProblemDetails architecture!
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    context.HttpContext.Response.ContentType = "application/problem+json";

                    var problem = new ProblemDetails
                    {
                        Status = StatusCodes.Status429TooManyRequests,
                        Title = "Too many requests",
                        Detail = "You have exceeded your rate limit. Please try again later.",
                        Type = nameof(ProblemDetails429ErrorTypes.RateLimit_Exceeded) // Assuming you have this enum
                    };

                    await context.HttpContext.Response.WriteAsJsonAsync(problem, token);
                };

                // 3. Define your policies

                // Policy A: Strict limit for Login/Auth endpoints (Prevents brute force)
                options.AddPolicy("AuthLimiter", httpContext =>
                {
                    var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: ip,
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 20,                 // Max 20 requests allowed...
                            Window = TimeSpan.FromMinutes(1), // ...in a total 1-minute window
                            SegmentsPerWindow = 4,            // Split that minute into four 15-second blocks
                            QueueLimit = 0,                   // Reject instantly (no waiting queue)
                            AutoReplenishment = true          // Automatically release expired segments
                        });
                });

                // Policy B: General limit for the rest of the API (e.g., 100 requests per minute)
                options.AddPolicy("GeneralApiLimiter", httpContext =>
                {
                    string? userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    string ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    return RateLimitPartition.GetSlidingWindowLimiter(
                            partitionKey: userId ?? ip,
                            factory: _ => new SlidingWindowRateLimiterOptions
                            {
                                PermitLimit = 100,                // Max 100 requests allowed...
                                Window = TimeSpan.FromMinutes(1), // ...in a total 1-minute window
                                SegmentsPerWindow = 4,            // Split that minute into four 15-second blocks
                                QueueLimit = 0,                   // Reject instantly (no waiting queue)
                                AutoReplenishment = true          // Automatically release expired segments
                            });
                });
            });

            return builder;
        }
    }
}