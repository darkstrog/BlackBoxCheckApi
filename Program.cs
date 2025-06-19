
using BlackBoxCheckApi.Models.Profiles;
using BlackBoxCheckApi.Models.Repository;
using BlackBoxCheckApi.Models;
using BlackBoxCheckApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace BlackBoxCheckApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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
                                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                                    ValidAudience = builder.Configuration["Jwt:Issuer"],
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                                };
            });

            var _connectionString =
                   builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string"
                       + "'DefaultConnection' not found.");

            var _connectionStringUsers =
                   builder.Configuration.GetConnectionString("AccountsConnection")
                       ?? throw new InvalidOperationException("Connection string"
                       + "'DefaultConnection' not found.");

            builder.Services.AddControllers();
            
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BlackBoxCheck API", Version = "v1" });
                
                c.SwaggerDoc("auth", new OpenApiInfo { Title = "Auth API", Version = "1.0" });

                c.SchemaFilter<GuidSchemaFilter>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        new List<string>()
                    }
                });

                c.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (docName == "auth")
                    {
                        return apiDesc.RelativePath?.StartsWith("api/auth") == true;
                    }

                    var versionPrefix = $"api/{docName}/";
                    return apiDesc.RelativePath?.StartsWith(versionPrefix) == true;
                });


                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });

            builder.Services.AddDbContext<UsersContext>(options => options.UseSqlite(_connectionStringUsers));
            builder.Services.AddDbContext<BlackBoxDbContext>(options => options.UseSqlite(_connectionString));

            builder.Services.AddAuthorization();

            builder.Services.AddScoped(typeof(IRepository<>), typeof(LiteRepositoryAsync<>));
            builder.Services.AddScoped<IQRService, LocalQRlibService>();
            builder.Services.AddScoped<BoxedItemsService>();
            builder.Services.AddScoped<ItemsListService>();
            builder.Services.AddScoped<AuthService>();

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("registration", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new()
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(10)
                        }));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlackBoxCheck V1");
                    c.SwaggerEndpoint("/swagger/auth/swagger.json", "Auth API");
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
