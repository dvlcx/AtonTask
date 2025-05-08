using System.Runtime.InteropServices;
using System.Text;
using AtonTask.DAL;
using AtonTask.DAL.Repositories;
using BLL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AtonTask;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //i use arch btw
        builder.Services.AddDbContext<TaskDbContext>(options => 
            options.UseSqlite(builder.Configuration.GetConnectionString(
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                "DefaultConnectionW" : "DefaultConnectionL"
            )));
        builder.Services.AddControllers();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => 
                policy.RequireClaim("IsAdmin", "true"));
            
            options.AddPolicy("AdminOrSelf", policy =>
                policy.RequireAssertion(context =>
                {
                    var isAdmin = context.User.HasClaim(c => 
                        c.Type == "IsAdmin" && c.Value == "true");
                    
                    var routeData = context.Resource as HttpContext;
                    var loginFromRoute = routeData?.Request.RouteValues["login"]?.ToString();
                    
                    var userLogin = context.User.FindFirst("Login")?.Value;
                    
                    return isAdmin || (userLogin != null && userLogin == loginFromRoute);
                }));
        });
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Aton Task API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT"
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
                    Array.Empty<string>()
                }
            });
        });
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<AuthService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(opt => opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Aton Task API")); 
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
