using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public class SQLStoneRepository : IStoneRepository
    {
        private readonly JewelrySalesSystemDbContext _context;

        public SQLStoneRepository(JewelrySalesSystemDbContext context)
        {
            _context = context;
        }

        public async Task<Stone> CreateAsync(Stone stone)
        {
            await _context.Stones.AddAsync(stone);
            await _context.SaveChangesAsync();
            return stone;
        }

        public async Task<Stone?> DeleteStoneAsync(int id)
        {
            var existingItem = await _context.Stones.FirstOrDefaultAsync(x => x.StoneId == id);
            if (existingItem == null)
            {
                return null;
            }
            existingItem.Status = "inactive";
            await _context.SaveChangesAsync();
            return existingItem;
        }

        public async Task<List<Stone>> GetAllStonesAsync()
        {

            var stone = _context.Stones.Where(x => x.Status == "active").AsQueryable();
            return await stone.ToListAsync();
        }


        public async Task<Stone?> GetStoneByIdAsync(int id)
        {
            return await _context.Stones.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.StoneId == id);
        }

    

   

        public async Task<Stone?> UpdateStoneAsync(int id, Stone stone)
        {
            var existingItem = await _context.Stones.FirstOrDefaultAsync(x => x.StoneId == id);
            if (existingItem == null)
            {
                return null;
            }
            existingItem.ProductId = stone.ProductId;
            existingItem.Name = stone.Name;
            existingItem.Type = stone.Type;
            existingItem.Price = stone.Price;
            existingItem.Color = stone.Color;
            existingItem.Status = stone.Status;
            existingItem.IsPrimary = stone.IsPrimary;

            await _context.SaveChangesAsync();
            return existingItem;
        }
    }
}