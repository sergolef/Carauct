using AuctionService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddDbContext<AuctionDbContext>(opt => {
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//add automapers that configured in RequestHelpers -> MappingProfiles
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//add masstransit (rebbitMQ)
builder.Services.AddMassTransit(x => 
{
    x.UsingRabbitMq((context, cfg) => {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

//seeding test data
try
{
    DbInitializer.InitDb(app);
}
catch(Exception e)
{
    Console.WriteLine(e);
}

app.Run();


