using Microsoft.EntityFrameworkCore;
using WebBanVang.Data;
using WebBanVang.Models.Domain;

namespace WebBanVang.Repository
{
    public class SQLGoldTypeRepository : IGoldTypeRepository
    {
        private readonly JewelrySalesSystemDbContext dbContext;

        public SQLGoldTypeRepository(JewelrySalesSystemDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<GoldType> AddGoldType(GoldType goldType )
        {
            await dbContext.GoldTypes.AddAsync(goldType);
            await dbContext.SaveChangesAsync();
            return goldType;
        }

        public async Task<GoldType> DeleteGoldType(int id)
        {
            var checkExist = await dbContext.GoldTypes.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.GoldId == id);
            if (checkExist == null) { return null; }
            checkExist.Status = "inactive";
            await dbContext.SaveChangesAsync();
            return checkExist;
        }

        public async Task<List<GoldType>> GetAllGoldType()
        {
            return await dbContext.GoldTypes.Where(x => x.Status == "active").ToListAsync();
        }

        public async Task<GoldType> GetGoldTypeById(int id)
        {
            return await dbContext.GoldTypes.Where(x => x.Status == "active").FirstOrDefaultAsync(x => x.GoldId == id);
        }

        public async Task<GoldType> UpdateGoldTYpe(int id, GoldType goldType)
        {
            var Checkexist = await dbContext.GoldTypes.FirstOrDefaultAsync(x => x.GoldId == id);
            if (Checkexist == null) return null;
            Checkexist.GoldName = goldType.GoldName;
            Checkexist.BuyPrice = goldType.BuyPrice;
            Checkexist.SellPrice = goldType.SellPrice;
            Checkexist.UpdateTime = goldType.UpdateTime;
            Checkexist.Status = goldType.Status;
            await dbContext.SaveChangesAsync();
            return Checkexist;
        }
    }
}
