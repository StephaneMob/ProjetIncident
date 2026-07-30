using System.Collections.Generic;
using System.Threading.Tasks;
using CICertSOAR.Models;

namespace CICertSOAR.Services
{
    public interface IIncidentService
    {
        Task<DashboardViewModel> GetDashboardMetricsAsync();
        Task<List<Incident>> GetIncidentsAsync(string? search, string? status, string? severity, int? sectorId, int? ministryId);
        Task<Incident?> GetIncidentByIdAsync(int id);
        Task<bool> UpdateIncidentStatusAsync(int id, string newStatus, string? notes);
    }
}
