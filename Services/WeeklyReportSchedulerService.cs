using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CICertSOAR.Models;

namespace CICertSOAR.Services
{
    public class WeeklyReportSchedulerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<WeeklyReportSchedulerService> _logger;

        public WeeklyReportSchedulerService(
            IServiceProvider serviceProvider,
            IWebHostEnvironment env,
            ILogger<WeeklyReportSchedulerService> logger)
        {
            _serviceProvider = serviceProvider;
            _env = env;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Ensures weekly report directory exists
            var reportDir = Path.Combine(_env.WebRootPath, "reports", "weekly");
            if (!Directory.Exists(reportDir))
            {
                Directory.CreateDirectory(reportDir);
            }

            // Immediately generate initial weekly report if none exists
            await GenerateWeeklyReportFileAsync(reportDir);

            while (!stoppingToken.IsCancellationRequested)
            {
                var now = DateTime.Now;
                // Calculate next Monday at 07:00 AM
                int daysUntilMonday = ((int)DayOfWeek.Monday - (int)now.DayOfWeek + 7) % 7;
                if (daysUntilMonday == 0 && now.Hour >= 7)
                {
                    daysUntilMonday = 7;
                }

                var nextRun = now.Date.AddDays(daysUntilMonday).AddHours(7);
                var delay = nextRun - now;

                _logger.LogInformation($"[Scheduler CI-CERT] Prochaine génération du rapport hebdomadaire prévue le : {nextRun:dd/MM/yyyy HH:mm}");

                try
                {
                    await Task.Delay(delay, stoppingToken);
                    await GenerateWeeklyReportFileAsync(reportDir);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de la génération automatique du rapport hebdomadaire du lundi 07h00.");
                }
            }
        }

        private async Task GenerateWeeklyReportFileAsync(string reportDir)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var reportingService = scope.ServiceProvider.GetRequiredService<IReportingService>();
                var filter = new ReportFilterViewModel
                {
                    StartDate = DateTime.Now.AddDays(-7),
                    EndDate = DateTime.Now,
                    Format = "PDF"
                };

                var pdfBytes = await reportingService.ExportPdfAsync(filter);
                var filePath = Path.Combine(reportDir, "Rapport_Hebdomadaire_CI-CERT.pdf");
                await File.WriteAllBytesAsync(filePath, pdfBytes);
                _logger.LogInformation($"[Scheduler CI-CERT] Rapport hebdomadaire généré et sauvegardé avec succès dans : {filePath}");
            }
        }
    }
}
