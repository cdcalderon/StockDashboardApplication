using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Migrations;
using StockDashboard.Core.Model;
namespace StockDashboard.Data
{
    public class StockDashboardMigrationsConfiguration : DbMigrationsConfiguration<StockDashboardContext>
    {
        public StockDashboardMigrationsConfiguration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;// NOTE: Remove in Production
        }

//        protected override void Seed(StockDashboardContext context)
//        {
//            // This runs when the server is initilized
//            base.Seed(context);
//#if DEBUG
//            if (context.Stocks.Count() == 0)
//            {
//                var stock = new Stock()
//                {
//                    AverageVolume = 5000000,
//                    Open = 34,
//                    DayHigh = 35,
//                    DayLow = 33
//                };

//                context.Stocks.Add(stock);

//                try
//                {
//                    context.SaveChanges();
//                }
//                catch (Exception ex)
//                {

//                    var msg = ex.Message;
//                }
//            }
//#endif
//        }
    }
}
