using StockDashboard.Core.Interfaces;
using StockDashboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDashboard.Data
{
    public class StockDashboardContext : DbContext
    {
        //public StockDashboardContext()
        //    : base("StockDashboard")
        //{
        //    Database.SetInitializer<StockDashboardContext>(null);
        //}

        public StockDashboardContext()
            : base(nameOrConnectionString: "StockDashboardDB") { }

        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Exchange> Exchanges { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Disable proxy creation and lazy loading; not wanted in this service context.
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<StockDashboardContext, StockDashboardMigrationsConfiguration>());

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Ignore<IIdentifiableEntity>();

            modelBuilder.Entity<Stock>()
            .HasRequired(x => x.Exchange)
            .WithRequiredDependent()
            .WillCascadeOnDelete(true);

        }
    }
}
