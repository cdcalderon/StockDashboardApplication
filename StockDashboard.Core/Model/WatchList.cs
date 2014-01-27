using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDashboard.Core.Model
{
    public class WatchList
    {
        public WatchList()
        {
            Stocks = new HashSet<Stock>();
        }

        // Primitive properties
        public int Id { get; set; }
        public string Title { get; set; }

        // Navigation properties
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
