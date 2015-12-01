using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace WebProject.Redirects
{
    public class RedirectDbContext : DbContext
    {
        public virtual IDbSet<RedirectRule> RedirectRules { get; set; }

        public RedirectDbContext() : base("EPiServerDB")
        {
            Database.SetInitializer<RedirectDbContext>(null);
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

}
