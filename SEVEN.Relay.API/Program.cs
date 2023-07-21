using Microsoft.EntityFrameworkCore;
using SEVEN.Relay.API.Data.Contexts;
using SEVEN.Relay.API.Data.Repositories;
using SEVEN.Relay.API.Endpoints;
using SEVEN.MissionControl.API.Client.DependencyInjection;
using SEVEN.Relay.API.BackgroundServices;
using SEVEN.Rover.Core.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<RelayContext>(options => options.UseInMemoryDatabase("RelayContextDB"));
builder.Services.AddScoped<IBroadcastReceiverRepository, BroadcastReceiverRepository>();

//DependencyInjection
builder.Services.AddRoverClient(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddAPIClient(builder.Configuration);

//BackgroundServices
builder.Services.AddHostedService<RelayService>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGroup("/messages").MessagesGroup();

app.Run();




/* Von SEVEN.Relay.Service (Alter Service)
 
 appsettings.json:
  "APIConnection": {
    "BaseUrl": "https://localhost:7217/"
  },
  "RoverConnection": {
    "RoverUrl": "http://192.168.178.37/"
  }
 
 *
 * public class Program
{
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                services.AddRoverClient(configuration, hostContext.HostingEnvironment.IsDevelopment());
                services.AddAPIClient(configuration);
                services.AddHostedService<RelayService>();
            })
            .Build();

        host.Run();
    }
}
*/