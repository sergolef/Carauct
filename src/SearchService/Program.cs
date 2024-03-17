using System.Net;
using MassTransit;
using MongoDB.Driver;
using MongoDB.Entities;
using Polly;
using Polly.Extensions.Http;
using SearchService.Consumers;
using SearchService.Data;
using SearchService.Models;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
//add automapper configuration
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpClient<AuctionSvcHttpClient>()
    .AddPolicyHandler(GetPolicy());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//add masstransit (rebbitMQ)
builder.Services.AddMassTransit(x => 
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();
    x.AddConsumersFromNamespaceContaining<AuctionUpdatedConsumer>();
    x.AddConsumersFromNamespaceContaining<AuctionDeletedConsumer>();

    x.SetEndpointNameFormatter( new KebabCaseEndpointNameFormatter("search", false));

    x.UsingRabbitMq((context, cfg) => {

         cfg.Host(builder.Configuration["RebbitMq:Host"], "/", host => {
            host.Username(builder.Configuration.GetValue("RebbitMq:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RebbitMq:Password", "guest"));
        });

        // retry
        cfg.ReceiveEndpoint("search-auction-created", e => {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("search-auction-deleted", e => {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionDeletedConsumer>(context);
        });

        cfg.ReceiveEndpoint("search-auction-updated", e => {
            e.UseMessageRetry(r => r.Interval(5, 5));
            e.ConfigureConsumer<AuctionUpdatedConsumer>(context);
        });


        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


//run service even waiting the polly
//it remove dependency on auction service
app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitializer.InitDb(app);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
});



app.Run();

//retry forever to get data until success every 3 seconds
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
