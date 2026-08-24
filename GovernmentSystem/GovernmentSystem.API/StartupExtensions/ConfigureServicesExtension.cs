using FluentValidation;
using FluentValidation.AspNetCore;
using GovernmentSystem.API.Application.CLI;
using GovernmentSystem.API.Application.Exceptions;
using GovernmentSystem.API.Application.Helpers;
using GovernmentSystem.API.Application.RequestDTOs.AdminDTOs;
using GovernmentSystem.API.Application.ResponseDTOs.Admin;
using GovernmentSystem.API.Application.Services;
using GovernmentSystem.API.Application.ServicesContracts;
using GovernmentSystem.API.Domain.Entities;
using GovernmentSystem.API.Domain.RepositoryContracts;
using GovernmentSystem.API.Domain.ResultErrorDomain;
using GovernmentSystem.API.Infrastructure.DbContext;
using GovernmentSystem.API.Infrastructure.Repositories;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace GovernmentSystem.API.StartupExtensions
{
    public static class ConfigureServicesExtension
    {
        /// <summary>
        /// Master extension method that calls all segregated configuration groups.
        /// Call this once in Program.cs via: builder.ConfigureAllServices();
        /// </summary>
        public static WebApplicationBuilder ConfigureAllServices(this WebApplicationBuilder builder)
        {
            builder.ConfigureDatabase()
                   .ConfigureIdentity()
                   .ConfigureCookies()
                   .ConfigureControllers()
                   .ConfigureExceptions()
                   .ConfigureSwagger()
                   .ConfigureCors()
                   .ConfigureApplicationServices()
                   .ConfigureValidation()
                   .ConfigureSensitiveDataServices()
                   .ConfigureRateLimiting()
                   ;

            return builder;
        }

        public static WebApplicationBuilder ConfigureDatabase(this WebApplicationBuilder builder)
        {
            // Get the connection string from appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException
                ("Connection string 'DefaultConnection' not found.");

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
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false; // Set to false if you want to allow only letters and digits
                options.Password.RequiredLength = 4;
                options.Password.RequiredUniqueChars = 1; // You can increase for stricter policy
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                ;

            return builder;
        }

        public static WebApplicationBuilder ConfigureCookies(this WebApplicationBuilder builder)
        {
            // Cookie configuration with ProblemDetails for 401/403
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // A. Security Settings
                options.Cookie.HttpOnly = true; // Prevents JavaScript from reading the cookie
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Requires HTTPS
                options.Cookie.SameSite = SameSiteMode.None; // strict prevents CSRF
                options.Cookie.Name = "__Host-Gov-Auth";

                options.Events.OnRedirectToLogin = context =>
                {
                    if (IsApiRequest(context.Request))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/problem+json";

                        var problem = new ProblemDetails
                        {
                            Status = StatusCodes.Status401Unauthorized,
                            Title = "Unauthorized",
                            Detail = "Authentication is required to access this resource.",
                            Type = nameof(ProblemDetails401ErrorTypes.Auth_UnauthorizedAccess)
                        };

                        return context.Response.WriteAsJsonAsync(problem);
                    }

                    // Non-API (if you ever have MVC pages)
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    if (IsApiRequest(context.Request))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/problem+json";

                        var problem = new ProblemDetails
                        {
                            Status = StatusCodes.Status403Forbidden,
                            Title = "Forbidden",
                            Detail = "You do not have permission to access this resource.",
                            Type = nameof(ProblemDetails403ErrorTypes.Auth_ForbiddenAccess)
                        };

                        return context.Response.WriteAsJsonAsync(problem);
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };

                options.ExpireTimeSpan = TimeSpan.FromDays(14); // 14 days
                options.SlidingExpiration = true; // optional, for sliding window

                //options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                //options.SlidingExpiration = true;
            });

            static bool IsApiRequest(HttpRequest request)
            {
                return request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase);
            }

            return builder;
        }

        public static WebApplicationBuilder ConfigureControllers(this WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddProblemDetails(); // Required for .NET 8 Exception Handler to work

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
            builder.Services.AddSwaggerGen(c =>
            {
                // Standard Swagger metadata
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Government System API", Version = "v1" });

                // 1. DEFINITION: "Here is a security scheme that exists"
                // This tells Swagger: "I support a security mode called 'ApiKey'.
                // It works by sending a value in the Header named 'X-Gov-Api-Key'."

                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "Enter your API Key below.",
                    Name = "X-Gov-Api-Key",       // The actual HTTP Header name to send
                    In = ParameterLocation.Header,// Where to put the key (Header, Query, Cookie)
                    Type = SecuritySchemeType.ApiKey, // The type of auth
                    Scheme = "ApiKeyScheme"
                });

                // 2. REQUIREMENT: "Apply this scheme to the endpoints"
                // This tells Swagger: "By default, assume every endpoint might need this lock."
                // When you click 'Authorize' and enter the key, Swagger will send that key
                // with EVERY request you make in the browser.

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey" // Must match the name defined in AddSecurityDefinition
                            },
                            In = ParameterLocation.Header
                        },
                        new List<string>() // Scopes (used for OAuth, empty for ApiKey)
                    }
                });

                // 1. Get the name of the generated XML file (usually YourProjectName.xml)
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

                // 2. Combine with the base directory to get the full path
                var fullPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

                // 3. Tell Swagger to use it
                c.IncludeXmlComments(fullPath);
            });

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

        public static WebApplicationBuilder ConfigureApplicationServices(this WebApplicationBuilder builder)
        {
            // --- REGISTER THE UNIT OF WORK ---
            // We use AddScoped for the lifetime. This means a single instance of UnitOfWork
            // (and therefore ApplicationDbContext) is created for each HTTP request. This is the standard.
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // --- REGISTER REPOSITORIES ---
            builder.Services.AddScoped<ICandidateRepository, CandidateRepository>();
            builder.Services.AddScoped<IVoterRepository, VoterRepository>();
            builder.Services.AddScoped<ICitizenRepository, CitizenRepository>();
            builder.Services.AddScoped<IAdminServices, AdminServices>();
            builder.Services.AddScoped<IVoterServices, VoterServices>();
            builder.Services.AddScoped<ICandidateServices, CandidateServices>();
            builder.Services.AddScoped<ICitizenServices, CitizenServices>();

            // Inside ConfigureApplicationServices extension:
            builder.Services.AddScoped<DataSeederCLI>(); // Don't forget to register it here!

            return builder;
        }

        public static WebApplicationBuilder ConfigureValidation(this WebApplicationBuilder builder)
        {
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<LoginDTO>();
            builder.Services.AddFluentValidationRulesToSwagger();

            return builder;
        }

        public static WebApplicationBuilder ConfigureSensitiveDataServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<SensitiveDataHelper>();

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

                options.AddPolicy("ApiKeyLimiter", httpContext =>
                {
                    //Try to get the API Key (matching your ApiKeyAuthAttribute!)
                    string? apiKey = httpContext.Request.Headers["X-Gov-Api-Key"].FirstOrDefault();

                    //Get the IP Address as a fallback
                    string ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                    // Apply the limit to whichever key we just determined
                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: apiKey ?? ip,
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 400,                 // Max 400 requests allowed...
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