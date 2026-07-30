using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using CICertSOAR.Models;
using CICertSOAR.Services;

namespace CICertSOAR.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IReportingService _reportingService;
        private readonly IAssetService _assetService;
        private readonly IWebHostEnvironment _env;

        public ReportsController(
            IReportingService reportingService, 
            IAssetService assetService,
            IWebHostEnvironment env)
        {
            _reportingService = reportingService;
            _assetService = assetService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Sectors = await _assetService.GetSectorsAsync();
            ViewBag.Ministries = await _assetService.GetMinistriesAsync();

            var model = new ReportFilterViewModel
            {
                StartDate = DateTime.Now.AddDays(-7),
                EndDate = DateTime.Now,
                Format = "PDF"
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Export(ReportFilterViewModel filter)
        {
            var fileNameDate = DateTime.Now.ToString("yyyy-MM-dd_HHmm");
            
            if (filter.Format.ToUpper() == "CSV")
            {
                var fileBytes = await _reportingService.ExportCsvAsync(filter);
                return File(fileBytes, "text/csv", $"Rapport_CI-CERT_{fileNameDate}.csv");
            }
            else if (filter.Format.ToUpper() == "WORD")
            {
                var fileBytes = await _reportingService.ExportWordAsync(filter);
                return File(fileBytes, "application/msword", $"Rapport_CI-CERT_{fileNameDate}.doc");
            }
            else // PDF
            {
                var fileBytes = await _reportingService.ExportPdfAsync(filter);
                return File(fileBytes, "application/pdf", $"Rapport_CI-CERT_{fileNameDate}.pdf");
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadWeeklyReport(string format = "PDF")
        {
            // Checks for auto-generated Monday 07:00 AM file stored locally on server
            var storedFilePath = Path.Combine(_env.WebRootPath, "reports", "weekly", "Rapport_Hebdomadaire_CI-CERT.pdf");
            if (format.ToUpper() == "PDF" && System.IO.File.Exists(storedFilePath))
            {
                var bytes = await System.IO.File.ReadAllBytesAsync(storedFilePath);
                return File(bytes, "application/pdf", $"Rapport_Hebdomadaire_CI-CERT_{DateTime.Now:yyyy-MM-dd}.pdf");
            }

            var filter = new ReportFilterViewModel
            {
                StartDate = DateTime.Now.AddDays(-7),
                EndDate = DateTime.Now,
                Format = format
            };

            return await Export(filter);
        }
    }
}
