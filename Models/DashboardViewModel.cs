using System.Collections.Generic;

namespace CICertSOAR.Models
{
    public class StatItem
    {
        public string Label { get; set; } = string.Empty;
        public int TotalCount { get; set; }
        public int UntreatedCount { get; set; }
        public int TreatedCount { get; set; }
    }

    public class DashboardViewModel
    {
        public int TotalIncidents { get; set; }
        public int UntreatedIncidents { get; set; } // Status: Détecté, Qualifié, Notifié
        public int TreatedIncidents { get; set; }   // Status: Résolu, Clos
        public int TotalAssets { get; set; }
        public int TotalOrganizations { get; set; }
        public double ResolutionRatePercentage { get; set; }

        public List<StatItem> IncidentsBySector { get; set; } = new();
        public List<StatItem> IncidentsByMinistry { get; set; } = new();

        public List<StatItem> MostVulnerableSectorsRanked { get; set; } = new();
        public List<StatItem> MostVulnerableMinistriesRanked { get; set; } = new();

        public List<Incident> RecentCriticalIncidents { get; set; } = new();
    }
}
