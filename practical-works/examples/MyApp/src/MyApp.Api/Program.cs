using Microsoft.Extensions.Options;
using MyApp.Application.Orders;
using MyApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddInfrastructure();

builder.Services.Configure<OrderOptions>(
    builder.Configuration.GetSection(OrderOptions.SectionName));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapGet("/api/diagnostics/options", (IOptions<OrderOptions> options) => options.Value);

app.Run();
