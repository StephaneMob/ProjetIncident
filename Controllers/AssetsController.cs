using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CICertSOAR.Services;

namespace CICertSOAR.Controllers
{
    public class AssetsController : Controller
    {
        private readonly IAssetService _assetService;

        public AssetsController(IAssetService assetService)
        {
            _assetService = assetService;
        }

        public async Task<IActionResult> Index(string? search, string? criticality, string? type, int? sectorId, int? ministryId, int page = 1)
        {
            ViewBag.Search = search;
            ViewBag.SelectedCriticality = criticality;
            ViewBag.SelectedType = type;
            ViewBag.SelectedSectorId = sectorId;
            ViewBag.SelectedMinistryId = ministryId;

            ViewBag.Sectors = await _assetService.GetSectorsAsync();
            ViewBag.Ministries = await _assetService.GetMinistriesAsync();

            var assets = await _assetService.GetAssetsPaginatedAsync(search, criticality, type, sectorId, ministryId, pageIndex: page, pageSize: 8);
            return View(assets);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var asset = await _assetService.GetAssetByIdAsync(id);
            if (asset == null) return NotFound();
            return PartialView("_AssetDetailsPartial", asset);
        }
    }
}
