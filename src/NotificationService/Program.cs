using MassTransit;
using NotificationService.Consumers;
using NotificationService.Hubs;

var builder = WebApplication.CreateBuilder(args);

//add masstransit (rebbitMQ)
builder.Services.AddMassTransit(x => 
{

    Console.WriteLine("===>"+builder.Configuration["RebbitMq:Host"]);

    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("nt", false));

    x.UsingRabbitMq((context, cfg) => 
    {
        cfg.Host(builder.Configuration["RebbitMq:Host"], "/", host => {
            host.Username(builder.Configuration.GetValue("RebbitMq:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RebbitMq:Password", "guest"));
        });
        
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<NotificationHub>("/notifications");

app.Run();
