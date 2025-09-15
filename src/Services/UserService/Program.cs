using Microsoft.EntityFrameworkCore;
using Consul;
using CCBR.Services.UserService;
using CCBR.Shared.ServiceDiscovery;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Business services
builder.Services.AddScoped<IUserService, UserService>();

// Infrastructure
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(config => config.Address = new Uri("http://consul:8500")));
builder.Services.AddSingleton<IServiceDiscovery, ConsulServiceDiscovery>();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();
app.MapHealthChecks("/health");

// Register with service discovery
var serviceDiscovery = app.Services.GetRequiredService<IServiceDiscovery>();
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStarted.Register(async () =>
    await serviceDiscovery.RegisterServiceAsync(
        "user-service",
        "http://user-service:5001",
        "http://user-service:5001/health"
    ));

app.Run();
