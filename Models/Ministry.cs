namespace CICertSOAR.Models
{
    public class Ministry
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        public int SectorId { get; set; }
        public Sector? Sector { get; set; }

        public ICollection<Organization> Organizations { get; set; } = new List<Organization>();
    }
}
