using System.Collections.Generic;
using System.Threading.Tasks;
using CICertSOAR.Models;

namespace CICertSOAR.Services
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }

    public interface IAssetService
    {
        Task<PaginatedList<Asset>> GetAssetsPaginatedAsync(string? search, string? criticality, string? type, int? sectorId, int? ministryId, int pageIndex = 1, int pageSize = 10);
        Task<Asset?> GetAssetByIdAsync(int id);
        Task<List<Sector>> GetSectorsAsync();
        Task<List<Ministry>> GetMinistriesAsync();
    }
}
