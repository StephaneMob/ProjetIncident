using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CICertSOAR.Services;

namespace CICertSOAR.Controllers
{
    public class IncidentsController : Controller
    {
        private readonly IIncidentService _incidentService;
        private readonly IAssetService _assetService;

        public IncidentsController(IIncidentService incidentService, IAssetService assetService)
        {
            _incidentService = incidentService;
            _assetService = assetService;
        }

        public async Task<IActionResult> Index(string? search, string? status, string? severity, int? sectorId, int? ministryId)
        {
            ViewBag.Search = search;
            ViewBag.SelectedStatus = status;
            ViewBag.SelectedSeverity = severity;
            ViewBag.SelectedSectorId = sectorId;
            ViewBag.SelectedMinistryId = ministryId;

            ViewBag.Sectors = await _assetService.GetSectorsAsync();
            ViewBag.Ministries = await _assetService.GetMinistriesAsync();

            var incidents = await _incidentService.GetIncidentsAsync(search, status, severity, sectorId, ministryId);
            return View(incidents);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var incident = await _incidentService.GetIncidentByIdAsync(id);
            if (incident == null) return NotFound();
            return PartialView("_IncidentDetailsPartial", incident);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status, string? notes)
        {
            var success = await _incidentService.UpdateIncidentStatusAsync(id, status, notes);
            if (!success) return BadRequest("Incident introuvable");
            return RedirectToAction(nameof(Index));
        }
    }
}
