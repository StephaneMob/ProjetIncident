using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CICertSOAR.Data;
using CICertSOAR.Models;

namespace CICertSOAR.Services
{
    public class AssetService : IAssetService
    {
        private readonly AppDbContext _context;

        public AssetService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedList<Asset>> GetAssetsPaginatedAsync(string? search, string? criticality, string? type, int? sectorId, int? ministryId, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.Assets
                .Include(a => a.Organization)
                    .ThenInclude(o => o!.Ministry)
                        .ThenInclude(m => m!.Sector)
                .Include(a => a.Incidents)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(a => 
                    a.Name.ToLower().Contains(s) || 
                    a.IpAddress.ToLower().Contains(s) || 
                    a.Domain.ToLower().Contains(s) ||
                    (a.Organization != null && a.Organization.Name.ToLower().Contains(s))
                );
            }

            if (!string.IsNullOrWhiteSpace(criticality))
            {
                query = query.Where(a => a.Criticality == criticality);
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(a => a.Type == type);
            }

            if (sectorId.HasValue && sectorId.Value > 0)
            {
                query = query.Where(a => a.Organization != null && a.Organization.Ministry != null && a.Organization.Ministry.SectorId == sectorId.Value);
            }

            if (ministryId.HasValue && ministryId.Value > 0)
            {
                query = query.Where(a => a.Organization != null && a.Organization.MinistryId == ministryId.Value);
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (pageIndex < 1) pageIndex = 1;
            if (pageIndex > totalPages) pageIndex = totalPages;

            var items = await query
                .OrderByDescending(a => a.DateRegistered)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<Asset>
            {
                Items = items,
                PageIndex = pageIndex,
                TotalPages = totalPages,
                TotalCount = totalCount
            };
        }

        public async Task<Asset?> GetAssetByIdAsync(int id)
        {
            return await _context.Assets
                .Include(a => a.Organization)
                    .ThenInclude(o => o!.Ministry)
                        .ThenInclude(m => m!.Sector)
                .Include(a => a.Incidents)
                    .ThenInclude(i => i.Vulnerability)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Sector>> GetSectorsAsync()
        {
            return await _context.Sectors.ToListAsync();
        }

        public async Task<List<Ministry>> GetMinistriesAsync()
        {
            return await _context.Ministries.Include(m => m.Sector).ToListAsync();
        }
    }
}
