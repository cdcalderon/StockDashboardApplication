using StockDashboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockDashboard.Core.Interfaces
{
    public interface IStockRepository : IDataRepository<Stock>
    {
        Stock GetSecurity(string symbol);
        List<TickerQuote> GetSecurityTickerQuotes();
        OperationStatus UpdateSecurities();
        OperationStatus InsertSecurityData();
        List<Stock> GetStockTickerQuotesFromProvider();
        List<Stock> GetStockTickerQuotes();
        List<Stock> GetTopThreeNetGainStocks();
    }
}
