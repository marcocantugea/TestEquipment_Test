using Microsoft.EntityFrameworkCore;
using TestEquipment_Test.Controllers;

namespace TestEquipment_Test.Models.Data
{
    public class BDContext : DbContext
    {
        public BDContext(DbContextOptions<BDContext> options) : base(options)
        {
        }
        public DbSet<Area_> Areas { get; set; }
        public DbSet<Line_> Lines { get; set; }
        public DbSet<Station_> Stations { get; set; }
        public DbSet<Plant_> Plants { get; set; }
        public DbSet<CheckOrder_> CheckOrder { get; set; }
        public DbSet<CheckIn_> CheckIns { get; set; }
        public DbSet<SerialNumber_> SerialNumber { get; set; }
    }
}
