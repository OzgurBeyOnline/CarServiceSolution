// Services/PageContentService.cs
using CarService.Web.Data;
using CarService.Web.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public class PageContentService : IPageContentService
    {
        private readonly ApplicationDbContext _context;
        public PageContentService(ApplicationDbContext context) 
            => _context = context;

        public async Task<IList<PageContent>> GetAllAsync()
            => await _context.PageContents
                             .OrderBy(pc => pc.DisplayOrder)
                             .AsNoTracking()
                             .ToListAsync();

        public async Task<PageContent?> GetByKeyAsync(string key)
            => await _context.PageContents
                             .AsNoTracking()
                             .FirstOrDefaultAsync(pc => pc.Key == key);

        public async Task UpdateAsync(PageContent entity)
        {
            _context.PageContents.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(int[] orderedIds)
        {
            var items = await _context.PageContents
                                    .Where(pc => orderedIds.Contains(pc.Id))
                                    .ToListAsync();
            for (int i = 0; i < orderedIds.Length; i++)
            {
                var pc = items.Single(x => x.Id == orderedIds[i]);
                pc.DisplayOrder = i;
            }
            await _context.SaveChangesAsync();
        }
    }
}
