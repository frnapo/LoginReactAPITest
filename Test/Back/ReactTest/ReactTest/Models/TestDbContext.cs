using System.Data.Entity;

namespace ReactTest.Models
{
    public partial class TestDbContext : DbContext
    {
        public TestDbContext()
            : base("name=DbContext")
        {
        }

        public virtual DbSet<Utenti> Utenti { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
