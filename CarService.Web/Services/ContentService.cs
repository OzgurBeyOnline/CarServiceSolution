using CarService.Web.Models;
using CarService.Web.Data;
using CarService.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace CarService.Web.Services
{
    public class ContentService : IContentService
    {
        private readonly ApplicationDbContext _db;
        public ContentService(ApplicationDbContext db) => _db = db;

        public async Task<List<PageContent>> GetAllAsync() =>
            await _db.PageContents
                     .OrderBy(c => c.DisplayOrder)
                     .ToListAsync();

        public async Task<PageContent?> GetByKeyAsync(string key) =>
            await _db.PageContents
                     .FirstOrDefaultAsync(c => c.Key == key);

        public async Task UpdateAsync(PageContent content)
        {
            _db.Update(content);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(int[] orderedIds)
        {
            for (int i = 0; i < orderedIds.Length; i++)
            {
                var c = await _db.PageContents.FindAsync(orderedIds[i]);
                if (c != null) c.DisplayOrder = i + 1;
            }
            await _db.SaveChangesAsync();
        }
    }
}
