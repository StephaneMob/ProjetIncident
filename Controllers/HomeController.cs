using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CICertSOAR.Services;

namespace CICertSOAR.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIncidentService _incidentService;

        public HomeController(IIncidentService incidentService)
        {
            _incidentService = incidentService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _incidentService.GetDashboardMetricsAsync();
            return View(model);
        }
    }
}
