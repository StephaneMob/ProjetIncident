using System.Threading.Tasks;
using CICertSOAR.Models;

namespace CICertSOAR.Services
{
    public interface IReportingService
    {
        Task<byte[]> ExportCsvAsync(ReportFilterViewModel filter);
        Task<byte[]> ExportWordAsync(ReportFilterViewModel filter);
        Task<byte[]> ExportPdfAsync(ReportFilterViewModel filter);
    }
}
