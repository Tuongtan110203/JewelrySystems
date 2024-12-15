using iText.Commons.Actions.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using WebBanVang.Data;
using WebBanVang.Models.Domain;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace WebBanVang.Repository
{
    public class SQLStoneRepository : IStoneRepository
    {
        private readonly JewelrySalesSystemDbContext _context;

        public SQLStoneRepository(JewelrySalesSystemDbContext context)
        {
            _context = context;
        }
        public async Task<Stone> GetStoneByCodeAsync(string stoneCode)
        {
            return await _context.Stones.FirstOrDefaultAsync(s => s.StoneCode == stoneCode);
        }
        

        public async Task<Stone> CreateAsync(Stone stone)
        {
            var stoneExist = await _context.Stones.AnyAsync(p => p.StoneCode == stone.StoneCode);
            if (stoneExist)
            {
                throw new ArgumentException("StoneCode already exists");
            }

            var productExist = await _context.Products.AnyAsync(c => c.ProductId == stone.ProductId);
            if (!productExist)
            {
                throw new ArgumentException("Invalid ProductCode");
            }

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

            var stone = _context.Stones.Where(x => x.Status == "active")
                                       .Include(x => x.Products) 
                                       .AsQueryable();
            return await stone.ToListAsync();
        }


        public async Task<Stone?> GetStoneByIdAsync(int id)
        {
            return await _context.Stones.Where(x => x.Status == "active")
                  .Include(x => x.Products).FirstOrDefaultAsync(x => x.StoneId == id);
        }

        public async Task<List<Stone>> GetStonesByStoneNameOrStoneCodeOrProductCodeAsync(string KeySearch)
        {
            return await _context.Stones.Where(x => x.Status == "active")
                .Include(x => x.Products)
                .Where(x => x.Name.Contains(KeySearch) || x.StoneCode.Contains(KeySearch) || x.Products.ProductCode.Contains(KeySearch)).ToListAsync();
        }

        public async Task<bool> IsStoneCodeExistsAsync(string stoneCode, int? id = null)
        {
            if (id.HasValue)
            {
                return await _context.Stones
                    .AnyAsync(c => c.StoneCode == stoneCode && c.ProductId != id.Value);
            }
            return await _context.Stones
               .AnyAsync(c => c.StoneCode == stoneCode);
        }

        public async Task<Stone?> UpdateStoneAsync(int id, Stone stone)
        {
            var existingItem = await _context.Stones.FirstOrDefaultAsync(x => x.StoneId == id);
            if (existingItem == null)
            {
                return null;
            }
        

            var productExist = await _context.Products.AnyAsync(c => c.ProductId == stone.ProductId);
            if (!productExist)
            {
                throw new ArgumentException("Invalid ProductCode");
            }
            existingItem.StoneCode = stone.StoneCode;
            existingItem.ProductId = stone.ProductId;
            existingItem.Name = stone.Name;
            existingItem.Price = stone.Price;
            existingItem.Color = stone.Color;
            existingItem.Status = stone.Status;

            await _context.SaveChangesAsync();
            return existingItem;
        }

        public async Task<Stone?> DetachStoneFromProductAsync(int id)
        {
            var stone = await _context.Stones.FirstOrDefaultAsync(x => x.StoneId == id);
            if (stone == null)
            {
                return null;
            }

           
            await _context.SaveChangesAsync();
            return stone;
        }
    }
}