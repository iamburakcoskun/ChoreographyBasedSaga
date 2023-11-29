using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

// Masstransit-RabbitMQ Configuration
builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(configuration.GetConnectionString("RabbitMQ"));
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlServer(configuration.GetConnectionString("SqlCon"))
);

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

app.UseAuthorization();

app.MapControllers();

app.Run();
