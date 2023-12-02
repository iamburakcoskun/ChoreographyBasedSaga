using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Models;
using Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

// Masstransit-RabbitMQ Configuration
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<PaymentCompletedEventConsumer>();
    config.AddConsumer<PaymentFailedEventConsumer>();
    config.AddConsumer<StockNotReservedEventConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(configuration.GetConnectionString("RabbitMQ"));

        cfg.ReceiveEndpoint(RabbitMQSettings.OrderPaymentCompletedEventQueueName,config =>
        {
            config.ConfigureConsumer<PaymentCompletedEventConsumer>(ctx);
        });

        cfg.ReceiveEndpoint(RabbitMQSettings.OrderPaymentFailedEventQueueName, config =>
        {
            config.ConfigureConsumer<PaymentFailedEventConsumer>(ctx);
        });

        cfg.ReceiveEndpoint(RabbitMQSettings.StockNotReservedEventQueueName, config =>
        {
            config.ConfigureConsumer<StockNotReservedEventConsumer>(ctx);
        });
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
