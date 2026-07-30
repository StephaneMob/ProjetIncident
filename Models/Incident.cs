using System;

namespace CICertSOAR.Models
{
    public class Incident
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty; // ex: RTIR-2026-0428

        public int AssetId { get; set; }
        public Asset? Asset { get; set; }

        public int VulnerabilityId { get; set; }
        public Vulnerability? Vulnerability { get; set; }

        public string Status { get; set; } = "Détecté"; // Détecté, Qualifié, Notifié, Résolu, Clos
        public string Severity { get; set; } = "Haute"; // Critique, Haute, Moyenne, Basse

        public DateTime DateDetected { get; set; } = DateTime.Now;
        public DateTime DateTicketCreated { get; set; } = DateTime.Now;
        public DateTime? DateEmailSent { get; set; }
        public DateTime? DateVulnerabilityFixed { get; set; }
        public DateTime? DateClosed { get; set; }

        public string FollowUpNotes { get; set; } = string.Empty;
        public string RemediationSteps { get; set; } = string.Empty;
    }
}
