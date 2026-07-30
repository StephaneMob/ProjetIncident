using System;

namespace CICertSOAR.Models
{
    public class Asset
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string Type { get; set; } = "Web"; // Web, Database, Infrastructure, API, Email
        public string Criticality { get; set; } = "Moyenne"; // Critique, Haute, Moyenne, Basse

        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public DateTime DateRegistered { get; set; } = DateTime.Now;

        public ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    }
}
