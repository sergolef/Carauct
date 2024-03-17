using AuctionService.Consumers;
using AuctionService.Data;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
   
    //use postgress db if servicebus out of service
    x.AddEntityFrameworkOutbox<AuctionDbContext>( o => {
        o.QueryDelay = TimeSpan.FromSeconds(10);

        o.UsePostgres();
        o.UseBusOutbox();
    });

    //add consumers
    x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
    
    x.SetEndpointNameFormatter( new KebabCaseEndpointNameFormatter("auction", false));

    x.UsingRabbitMq((context, cfg) => {
        cfg.Host(builder.Configuration["RebbitMq:Host"], "/", host => {
            host.Username(builder.Configuration.GetValue("RebbitMq:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RebbitMq:Password", "guest"));
        });
        
        cfg.ConfigureEndpoints(context);
    });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
        options.TokenValidationParameters.NameClaimType = "username";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

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


