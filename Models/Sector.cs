namespace CICertSOAR.Models
{
    public class Sector
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation
        public ICollection<Ministry> Ministries { get; set; } = new List<Ministry>();
    }
}
