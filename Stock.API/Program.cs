using MassTransit;
using Microsoft.EntityFrameworkCore;
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

    config.AddConsumer<PaymentFailedEventConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(configuration.GetConnectionString("RabbitMQ"));

        cfg.ReceiveEndpoint(RabbitMQSettings.StockOrderCreatedEventQueue, e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(ctx);
        });

        cfg.ReceiveEndpoint(RabbitMQSettings.StockPaymentFailedEventQueueName, e =>
        {
            e.ConfigureConsumer<PaymentFailedEventConsumer>(ctx);
        });
    });
});

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseInMemoryDatabase("StockDb")
);

var sp = builder.Services.BuildServiceProvider();

using (var scope = sp.CreateScope())
{
    var scopedServices = scope.ServiceProvider;
    var db = scopedServices.GetRequiredService<AppDbContext>();

    db.Database.EnsureDeleted();

    db.Database.EnsureCreated();

    InitializeDbForTests(db);

    void InitializeDbForTests(AppDbContext db)
    {
        var isAdded = db.Stock.Any();

        if (!isAdded)
        {
            List<Stock.API.Models.Stock> stockList = new()
            {
                new Stock.API.Models.Stock
                {
                    Id=1,
                    ProductId = 1,
                    Count = 100,
                },
                new Stock.API.Models.Stock
                {
                    Id = 2,
                    ProductId = 2,
                    Count = 100,
                }
            };

            foreach (var stock in stockList)
            {
                db.Stock.Add(stock);

            }

            var amountAdded = db.SaveChanges();
        }
    }
}


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
