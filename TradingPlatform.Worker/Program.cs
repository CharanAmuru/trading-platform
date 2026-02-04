using Microsoft.EntityFrameworkCore;
using TradingPlatform.Application.Interfaces;
using TradingPlatform.Infrastructure.Persistence;
using TradingPlatform.Infrastructure.Pricing;
using TradingPlatform.Infrastructure.Repositories;
using TradingPlatform.Worker.Workers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<TradingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TradingDb")));

builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddSingleton<IMarketPriceProvider, InMemoryMarketPriceProvider>();

builder.Services.AddHostedService<ExecutionWorker>();

var host = builder.Build();
host.Run();
