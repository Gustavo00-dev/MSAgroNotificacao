using MSAgroNotificacao;
using Microsoft.EntityFrameworkCore;
using MSAgroNotificacao.Data;

var builder = Host.CreateApplicationBuilder(args);

// Configure DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AgroDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
