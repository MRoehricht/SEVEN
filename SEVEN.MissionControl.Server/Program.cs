using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MudBlazor.Services;
using OpenIddict.Abstractions;
using SEVEN.MissionControl.Server.API.Endpoints;
using SEVEN.MissionControl.Server.Data.Contexts;
using SEVEN.MissionControl.Server.Data.Generators;
using SEVEN.MissionControl.Server.Data.Repositories;
using SEVEN.MissionControl.Server.Data.Repositories.Interfaces;
using SEVEN.MissionControl.Server.Options;
using SEVEN.MissionControl.Server.Services;
using SEVEN.MissionControl.Server.Services.Interfaces;
using Swashbuckle.AspNetCore.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;


namespace SEVEN.MissionControl.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenIddict(options => {
            options.AddClient(opt => {
                opt.AllowAuthorizationCodeFlow();

                opt.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                opt.UseAspNetCore()
                    .EnableRedirectionEndpointPassthrough();

                opt.UseAspNetCore().DisableTransportSecurityRequirement();

                opt.UseWebProviders()
                    .AddGitHub(github => {
                        github.SetClientId(builder.Configuration["GITHUB_CLIENT_ID"])
                            .SetClientSecret(builder.Configuration["GITHUB_CLIENT_SECRET"])
                            .SetRedirectUri("/authentication/webhook/oauth/github");
                    });
            });

            options.AddCore(opt => {
                opt.UseEntityFrameworkCore()
                    .UseDbContext<MissionControlContext>();
            });
        });

        builder.Services.AddDbContext<MissionControlContext>(options =>
        {
            options.UseInMemoryDatabase("MissionControlContextDB");
            options.UseOpenIddict();
        });

        //builder.Services.AddDbContext<MissionControlContext>(optionsAction =>
        //{
         //   var postgresHost = builder.Configuration["DB_HOST"];
          //  var postgresPort = builder.Configuration["DB_PORT"];
           // var postgresDatabase = builder.Configuration["DB_DB"];
            //var postgresUser = builder.Configuration["DB_USER"];
            //var postgresPassword = builder.Configuration["DB_PASSWORD"];
            //optionsAction.UseNpgsql($"host={postgresHost};port={postgresPort};database={postgresDatabase};username={postgresUser};password={postgresPassword};");
           // optionsAction.UseOpenIddict();
        //});

        var sevenOptions = new SEVENOptions();
        builder.Configuration.Bind(sevenOptions);
        builder.Services.Configure<SEVENOptions>(builder.Configuration);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddMudServices();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddTransient<IRoverTaskRepository, RoverTaskRepository>();
        builder.Services.AddTransient<IProbeRepository, ProbeRepository>();
        builder.Services.AddTransient<IMeasurementRepository, MeasurementRepository>();
        builder.Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();


        builder.Services.AddAuthorization();

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        
        builder.Services.AddAuthentication(o => {
                o.DefaultScheme = "Jwt_Or_Cookie";
                o.DefaultAuthenticateScheme = "Jwt_Or_Cookie";
            }).AddCookie(opt => {
                opt.Cookie.Name = "SEVEN";
                opt.Cookie.HttpOnly = true;
                opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                opt.Events.OnRedirectToLogin = context => {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                opt.Events.OnRedirectToAccessDenied = context => {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            })
            .AddJwtBearer(opt => {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = "SEVEN",
                    ValidAudience = "SEVEN",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sevenOptions.PROBE_SECRET)),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = OpenIddictConstants.Claims.Subject,
                    RoleClaimType = OpenIddictConstants.Claims.Role
                };
            })
            .AddPolicyScheme("Jwt_Or_Cookie", "Jwt_Or_Cookie", o => {
                o.ForwardDefaultSelector = context => {
                    if (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var authHeader) &&
                        authHeader.FirstOrDefault()?.StartsWith("Bearer ") is true) {
                        return JwtBearerDefaults.AuthenticationScheme;
                    }

                    return CookieAuthenticationDefaults.AuthenticationScheme;
                };
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Bearer eyJhbGciOiJIUzI1...\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        RoverGenerator.Initialize(builder.Services);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        else
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.MapGroup("/rover").RoverGroup();
        app.MapGroup("/tasks").RoverTaskGroup();
        app.MapGroup("/probe").ProbeGroup();
        app.MapGroup("/measurement").MeasurementGroup();
        app.MapGroup("/authentication").AuthenticationGroup();

        app.Run();
    }
}
