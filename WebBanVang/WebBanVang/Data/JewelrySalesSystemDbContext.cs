using Microsoft.EntityFrameworkCore;
using WebBanVang.Models.Domain;

namespace WebBanVang.Data
{
    public class JewelrySalesSystemDbContext : DbContext
    {
        public JewelrySalesSystemDbContext(DbContextOptions<JewelrySalesSystemDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<Warranty> Warranties { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<GoldType> GoldTypes { get; set; }
        public DbSet<Stone> Stones { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Warranty>(entity =>
            {
                entity.HasKey(w => w.WarrantyId);

                entity.HasOne(w => w.OrderDetails)
                    .WithMany()
                    .HasForeignKey(w => w.OrderDetailId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(w => w.Customers)
                    .WithMany()
                    .HasForeignKey(w => w.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
