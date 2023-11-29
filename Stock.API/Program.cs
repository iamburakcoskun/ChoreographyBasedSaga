using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared;
using Stock.API.Consumers;
using Stock.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

// Masstransit-RabbitMQ Configuration
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<OrderCreatedEventConsumer>();   

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(configuration.GetConnectionString("RabbitMQ"));

        cfg.ReceiveEndpoint(RabbitMQSettings.StockOrderCreatedEventQueue, e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(ctx);
        });
    });
});


// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseInMemoryDatabase("StockDb")
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
