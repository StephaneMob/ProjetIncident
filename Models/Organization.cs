namespace CICertSOAR.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int MinistryId { get; set; }
        public Ministry? Ministry { get; set; }

        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;

        public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }
}
