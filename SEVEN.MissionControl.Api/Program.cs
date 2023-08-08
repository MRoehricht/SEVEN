using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SEVEN.MissionControl.Api.AuthenticationSchemes;
using SEVEN.MissionControl.Api.Data.Contexts;
using SEVEN.MissionControl.Api.Data.Generators;
using SEVEN.MissionControl.Api.Data.Repositories;
using SEVEN.MissionControl.Api.Data.Repositories.Interfaces;
using SEVEN.MissionControl.Api.Features;
using SEVEN.MissionControl.Api.Options;

var builder = WebApplication.CreateBuilder(args);

const string sevenOrigins = "seven";
var allowedOrigins = builder.Configuration["ALLOWED_ORIGINS"]?.Split(',') ?? Array.Empty<string>();
builder.Services.AddCors(opt => {
    opt.AddPolicy(name: sevenOrigins, policyBuilder => {
        policyBuilder.WithOrigins(allowedOrigins)
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options => {
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedHost |
        ForwardedHeaders.XForwardedProto;

    options.ForwardLimit = 2;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddAuthentication(ApiKeyAuthenticationScheme.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationScheme>(ApiKeyAuthenticationScheme.SchemeName, null);

builder.Services.AddAuthorization(options => {
    options.AddPolicy("probe", policy => policy.RequireClaim("ProbeId"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options => {
        options.AddSecurityDefinition(ApiKeyAuthenticationScheme.SchemeName, new OpenApiSecurityScheme {
            Name = ApiKeyAuthenticationScheme.KeyHeaderName,
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement() {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {
                        Type = ReferenceType.SecurityScheme,
                        Id = ApiKeyAuthenticationScheme.SchemeName
                    },
                    In = ParameterLocation.Header,
                },
                new List<string>()
            }
        });
    });

builder.Services.AddDbContext<MissionControlContext>(optionsAction => {
    var postgresHost = builder.Configuration["DB_HOST"];
    var postgresPort = builder.Configuration["DB_PORT"];
    var postgresDatabase = builder.Configuration["DB_DB"];
    var postgresUser = builder.Configuration["DB_USER"];
    var postgresPassword = builder.Configuration["DB_PASSWORD"];
    optionsAction.UseNpgsql($"host={postgresHost};port={postgresPort};database={postgresDatabase};username={postgresUser};password={postgresPassword};");
});

var sevenOptions = new SEVENOptions();
builder.Configuration.Bind(sevenOptions);
builder.Services.Configure<SEVENOptions>(builder.Configuration);

builder.Services.AddTransient<IRoverTaskRepository, RoverTaskRepository>();
builder.Services.AddTransient<IProbeRepository, ProbeRepository>();
builder.Services.AddTransient<IMeasurementRepository, MeasurementRepository>();

RoverGenerator.Initialize(builder.Services);

var app = builder.Build();

app.UseForwardedHeaders();
app.UseCors(sevenOrigins);
app.UseRouting();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/rover").RoverGroup();
app.MapGroup("/tasks").RoverTaskGroup();
app.MapGroup("/probe").ProbeGroup();
app.MapGroup("/measurement").MeasurementGroup();
app.Run();