using Microsoft.EntityFrameworkCore;

namespace MSAgroNotificacao
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<MSAgroNotificacao.Data.AgroDbContext>();

                    var since = DateTime.UtcNow.AddHours(-24);

                    var talhoes = await db.Sensors
                        .Where(s => s.DataUltimaAtualizacao >= since)
                        .Select(s => s.IdTalhao)
                        .Distinct()
                        .ToListAsync(stoppingToken);
                    _logger.LogInformation("Processando {count} talhões com leituras recentes", talhoes.Count);
                    foreach (var idTalhao in talhoes)
                    {
                        var leituras = await db.Sensors
                            .Where(s => s.IdTalhao == idTalhao && s.DataUltimaAtualizacao >= since)
                            .ToListAsync(stoppingToken);

                        if (!leituras.Any())
                            continue;

                        var todosAbaixo = leituras.All(l => l.UmidadeSolo < 30m);

                        if (todosAbaixo)
                        {
                            var existeAlerta = await db.Alertas.AnyAsync(a => a.IdTalhao == idTalhao, stoppingToken);

                            if (!existeAlerta)
                            {
                                var alerta = new MSAgroNotificacao.Data.Alerta
                                {
                                    IdTalhao = idTalhao,
                                    DataAlerta = DateTime.UtcNow
                                };

                                db.Alertas.Add(alerta);
                                await db.SaveChangesAsync(stoppingToken);

                                _logger.LogInformation("Alerta criado para talhao {idTalhao} às {time}", idTalhao, DateTimeOffset.UtcNow);
                            }
                        }
                        else
                        {
                            _logger.LogInformation("Talhao {idTalhao} possui leituras de umidade acima do limite", idTalhao);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar leituras de sensores");
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
