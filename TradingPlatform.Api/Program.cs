using Microsoft.EntityFrameworkCore;
using TradingPlatform.Application.Commands.PlaceOrder;
using TradingPlatform.Application.Interfaces;
using TradingPlatform.Infrastructure.Persistence;
using TradingPlatform.Infrastructure.Pricing;
using TradingPlatform.Infrastructure.Repositories;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TradingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TradingDb")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IExecutionJobWriter, SqlExecutionJobWriter>();
builder.Services.AddScoped<PlaceOrderHandler>();

// ✅ Positions
builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddCors(o =>
{
    o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});



// ✅ Mock prices
builder.Services.AddSingleton<IMarketPriceProvider, InMemoryMarketPriceProvider>();

// ✅ CORS for frontend
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("frontend", p =>
        p.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseHttpsRedirection();

app.UseCors("frontend");

app.MapControllers();

app.Run();
