using Microsoft.EntityFrameworkCore;

namespace AdvertisementAPI
{
    public class AdContext : DbContext
    {
        public AdContext(DbContextOptions<AdContext> options) : base(options) { }
        public DbSet<ADInfo> Advertisements { get; set; }
        public DbSet<AdStatistic> ADStatistics { get; set; }
        public DbSet<Client> Clients { get; set; }
    }
}
