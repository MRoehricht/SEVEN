using Microsoft.EntityFrameworkCore;
using SEVEN.Relay.API.Data.Contexts;
using SEVEN.Relay.API.Data.Repositories;
using SEVEN.Relay.API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<RelayContext>(options => options.UseInMemoryDatabase("RelayContextDB"));
builder.Services.AddScoped<IBroadcastReceiverRepository, BroadcastReceiverRepository>();

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