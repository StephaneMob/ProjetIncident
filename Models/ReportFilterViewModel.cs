using System;

namespace CICertSOAR.Models
{
    public class ReportFilterViewModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? SectorId { get; set; }
        public int? MinistryId { get; set; }
        public string? Status { get; set; }
        public string? Severity { get; set; }
        public string Format { get; set; } = "PDF"; // PDF, CSV, Word
    }
}
