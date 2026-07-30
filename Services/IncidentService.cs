using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CICertSOAR.Data;
using CICertSOAR.Models;

namespace CICertSOAR.Services
{
    public class IncidentService : IIncidentService
    {
        private readonly AppDbContext _context;

        public IncidentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardMetricsAsync()
        {
            var incidents = await _context.Incidents
                .Include(i => i.Asset)
                    .ThenInclude(a => a!.Organization)
                        .ThenInclude(o => o!.Ministry)
                            .ThenInclude(m => m!.Sector)
                .Include(i => i.Vulnerability)
                .ToListAsync();

            var totalIncidents = incidents.Count;
            var untreatedIncidents = incidents.Count(i => i.Status == "Détecté" || i.Status == "Qualifié" || i.Status == "Notifié");
            var treatedIncidents = incidents.Count(i => i.Status == "Résolu" || i.Status == "Clos");
            var totalAssets = await _context.Assets.CountAsync();
            var totalOrganizations = await _context.Organizations.CountAsync();

            double resRate = totalIncidents > 0 ? Math.Round((double)treatedIncidents / totalIncidents * 100, 1) : 0;

            // Group by Sectors
            var sectorStats = incidents
                .Where(i => i.Asset?.Organization?.Ministry?.Sector != null)
                .GroupBy(i => i.Asset!.Organization!.Ministry!.Sector!.Name)
                .Select(g => new StatItem
                {
                    Label = g.Key,
                    TotalCount = g.Count(),
                    UntreatedCount = g.Count(i => i.Status == "Détecté" || i.Status == "Qualifié" || i.Status == "Notifié"),
                    TreatedCount = g.Count(i => i.Status == "Résolu" || i.Status == "Clos")
                })
                .OrderByDescending(s => s.TotalCount)
                .ToList();

            // Group by Ministries
            var ministryStats = incidents
                .Where(i => i.Asset?.Organization?.Ministry != null)
                .GroupBy(i => i.Asset!.Organization!.Ministry!.Name)
                .Select(g => new StatItem
                {
                    Label = g.Key,
                    TotalCount = g.Count(),
                    UntreatedCount = g.Count(i => i.Status == "Détecté" || i.Status == "Qualifié" || i.Status == "Notifié"),
                    TreatedCount = g.Count(i => i.Status == "Résolu" || i.Status == "Clos")
                })
                .OrderByDescending(m => m.TotalCount)
                .ToList();

            // Most vulnerable rankings (ranked by untreated incidents + total incidents)
            var mostVulnerableSectors = sectorStats.OrderByDescending(s => s.UntreatedCount).ThenByDescending(s => s.TotalCount).ToList();
            var mostVulnerableMinistries = ministryStats.OrderByDescending(m => m.UntreatedCount).ThenByDescending(m => m.TotalCount).ToList();

            var recentCritical = incidents
                .Where(i => i.Severity == "Critique" || i.Severity == "Haute")
                .OrderByDescending(i => i.DateDetected)
                .Take(5)
                .ToList();

            return new DashboardViewModel
            {
                TotalIncidents = totalIncidents,
                UntreatedIncidents = untreatedIncidents,
                TreatedIncidents = treatedIncidents,
                TotalAssets = totalAssets,
                TotalOrganizations = totalOrganizations,
                ResolutionRatePercentage = resRate,
                IncidentsBySector = sectorStats,
                IncidentsByMinistry = ministryStats,
                MostVulnerableSectorsRanked = mostVulnerableSectors,
                MostVulnerableMinistriesRanked = mostVulnerableMinistries,
                RecentCriticalIncidents = recentCritical
            };
        }

        public async Task<List<Incident>> GetIncidentsAsync(string? search, string? status, string? severity, int? sectorId, int? ministryId)
        {
            var query = _context.Incidents
                .Include(i => i.Asset)
                    .ThenInclude(a => a!.Organization)
                        .ThenInclude(o => o!.Ministry)
                            .ThenInclude(m => m!.Sector)
                .Include(i => i.Vulnerability)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(i => 
                    i.TicketNumber.ToLower().Contains(s) ||
                    (i.Asset != null && (i.Asset.Name.ToLower().Contains(s) || i.Asset.IpAddress.ToLower().Contains(s) || i.Asset.Domain.ToLower().Contains(s))) ||
                    (i.Vulnerability != null && (i.Vulnerability.CveId.ToLower().Contains(s) || i.Vulnerability.Title.ToLower().Contains(s)))
                );
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(i => i.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(severity))
            {
                query = query.Where(i => i.Severity == severity);
            }

            if (sectorId.HasValue && sectorId.Value > 0)
            {
                query = query.Where(i => i.Asset != null && i.Asset.Organization != null && i.Asset.Organization.Ministry != null && i.Asset.Organization.Ministry.SectorId == sectorId.Value);
            }

            if (ministryId.HasValue && ministryId.Value > 0)
            {
                query = query.Where(i => i.Asset != null && i.Asset.Organization != null && i.Asset.Organization.MinistryId == ministryId.Value);
            }

            return await query.OrderByDescending(i => i.DateDetected).ToListAsync();
        }

        public async Task<Incident?> GetIncidentByIdAsync(int id)
        {
            return await _context.Incidents
                .Include(i => i.Asset)
                    .ThenInclude(a => a!.Organization)
                        .ThenInclude(o => o!.Ministry)
                            .ThenInclude(m => m!.Sector)
                .Include(i => i.Vulnerability)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<bool> UpdateIncidentStatusAsync(int id, string newStatus, string? notes)
        {
            var incident = await _context.Incidents.FindAsync(id);
            if (incident == null) return false;

            incident.Status = newStatus;
            if (!string.IsNullOrWhiteSpace(notes))
            {
                incident.FollowUpNotes += $"\n[{DateTime.Now:dd/MM/yyyy HH:mm}] ({newStatus}): {notes}";
            }

            if (newStatus == "Notifié" && !incident.DateEmailSent.HasValue)
            {
                incident.DateEmailSent = DateTime.Now;
            }
            else if (newStatus == "Résolu" && !incident.DateVulnerabilityFixed.HasValue)
            {
                incident.DateVulnerabilityFixed = DateTime.Now;
            }
            else if (newStatus == "Clos")
            {
                if (!incident.DateVulnerabilityFixed.HasValue) incident.DateVulnerabilityFixed = DateTime.Now;
                incident.DateClosed = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
