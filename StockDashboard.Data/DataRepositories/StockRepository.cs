using StockDashboard.Core.Interfaces;
using StockDashboard.Core.Model;
using StockDashboard.Data.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace StockDashboard.Data
{
    public class StockRepository : DataRepositoryBase<Stock>, IStockRepository
    {
        bool localDataOnly = Boolean.Parse(ConfigurationManager.AppSettings["LocalDataOnly"]);
        //Some random symbols to use in order to get data into the database
        private readonly string[] _StockSymbols = {"BIIB", "CF", "CSCO", "PNRA", "EMC", "FDX", "GE", "H", "INTC", "JPM", "K", 
                                                   "LLY", "MSFT", "NKE", "ORCL", "PG", "Q", "RBS", "S", "T", "UL", "V", "WMT", 
                                                   "XRX", "YHOO", "CMG", "AAPL", "IBM", "NOK", "HRB", "FCX", "MTH", "SPF", 
                                                   "CRM", "CAT", "LMT", "GD", "XOM", "CVX", "SLB", "BA", "F", "X", "AA", 
                                                   "NOC", "AMZN","BAC", "GLD", "DIS", "GOOG", "EBAY", "AOL", "BIDU" };
        protected override Stock AddEntity(StockDashboardContext entityContext, Stock entity)
        {
            return entityContext.Stocks.Add(entity);
        }

        protected override Stock UpdateEntity(StockDashboardContext entityContext, Stock entity)
        {
            return (from e in entityContext.Stocks
                    where e.Id == entity.Id
                    select e).FirstOrDefault();
        }

        protected override IEnumerable<Stock> GetEntities(StockDashboardContext entityContext)
        {
            return from e in entityContext.Stocks
                   select e;
        }

        protected override Stock GetEntity(StockDashboardContext entityContext, int id)
        {
            var query = (from e in entityContext.Stocks
                        where e.Id == id
                        select e);

            var results = query.FirstOrDefault();

            return results;
        }

        
        public List<Stock> GetTopThreeNetGainStocks()
        {
            using (StockDashboardContext entityContext = new StockDashboardContext())
            {
                return entityContext.Stocks.Select(s =>
                   new Stock
                   {
                       Symbol = s.Symbol,
                       Change = s.Change,
                       Last = s.Last,
                       ImageSource = s.ImageSource
                   }).OrderBy(s => s.Change).Take(3).ToList();
            }
        }

        public Stock GetSecurity(string symbol)
        {
            using (StockDashboardContext entityContext = new StockDashboardContext())
            {
                var sec = entityContext.Stocks.Where(s => s.Symbol == symbol).SingleOrDefault();
                if (sec == null)
                {
                    var engine = new StockEngine();
                    sec = engine.GetSecurityQuotes(symbol).FirstOrDefault();
                    entityContext.Stocks.Add(sec);
                    var opStatus = Save(sec);
                    if (!opStatus.Status)
                    {
                        sec = new Stock { Company = "Error getting quote." };
                    }
                }
                return sec;
            }
        }

        public List<TickerQuote> GetSecurityTickerQuotes()
        {
            using (StockDashboardContext entityContext = new StockDashboardContext())
            {
                return entityContext.Stocks.Select(s =>
                    new TickerQuote
                    {
                        Symbol = s.Symbol,
                        Change = s.Change,
                        Last = s.Last
                    }).OrderBy(tq => tq.Symbol).ToList();
            }
        }

        public List<Stock> GetStockTickerQuotesFromProvider()
        {
            var engine = new StockEngine(); //NOTE: Refactor by injecting Stock Engine Provider 
            var stocks = engine.GetSecurityQuotesFromYahooFinance(_StockSymbols);  //From Yahoo
            return stocks;
        }

        public List<Stock> GetStockTickerQuotes()
        {
            using (StockDashboardContext entityContext = new StockDashboardContext())
            {
                return entityContext.Stocks.ToList();
            }
        }

        public OperationStatus UpdateSecurities()
        {
            var opStatus = new OperationStatus { Status = true };
            using (StockDashboardContext entityContext = new StockDashboardContext())
            {
                
                if (localDataOnly) return opStatus;

                var securities = entityContext.Stocks; //Get existing securities
                var engine = new StockEngine();
                var updatedSecurities = engine.GetSecurityQuotes(securities.Select(s => s.Symbol).ToArray());
                //Return if updatedSecurities is null
                if (updatedSecurities == null) return new OperationStatus { Status = false };

                foreach (var security in securities)
                {
                    //Grab updated version of security
                    var updatedSecurity = updatedSecurities.Where(s => s.Symbol == security.Symbol).Single();
                    security.Change = updatedSecurity.Change;
                    security.Last = updatedSecurity.Last;
                    security.PercentChange = updatedSecurity.PercentChange;
                    security.RetrievalDateTime = updatedSecurity.RetrievalDateTime;
                    security.Shares = updatedSecurity.Shares;
                    entityContext.Entry(security).State = System.Data.Entity.EntityState.Modified;
                }

                //Insert records
                try
                {
                    entityContext.SaveChanges();
                }
                catch (Exception exp)
                {
                    return OperationStatus.CreateFromException("Error updating security quote.", exp);
                }
            }
            return opStatus;
        }

        public OperationStatus InsertSecurityData()
        {
            var engine = new StockEngine();

            //var securities = engine.GetSecurityQuotes(_StockSymbols);  // Uncomment to Fetch Data From Google  
            
            var securities = engine.GetSecurityQuotesFromYahooFinance(_StockSymbols);  //From Yahoo

            var exchanges = securities.OfType<Stock>().Select(s => s.Exchange.Title).Distinct();

            if (securities != null && securities.Count > 0)
            {
                using (var ts = new TransactionScope())
                {
                    using (StockDashboardContext entityContext = new StockDashboardContext())
                    {
                        var opStatus = DeleteSecurityRecords(entityContext);
                        if (!opStatus.Status) return opStatus;

                        opStatus = InsertExchanges(exchanges, entityContext);
                        if (!opStatus.Status) return opStatus;

                        opStatus = InsertSecurities(securities, entityContext);
                        if (!opStatus.Status) return opStatus;
                    }
                    ts.Complete();
                }
            }
            return new OperationStatus { Status = true };
        }

        private static OperationStatus InsertSecurities(List<Stock> securities, StockDashboardContext context)
        {
            foreach (var security in securities)
            {
                //Update stock's exchange ID so we don't get dups
                if (security is Stock)
                {
                    var stock = (Stock)security;
                    stock.Exchange = context.Exchanges.Where(e => e.Title == stock.Exchange.Title).First();
                }
                //Add security into collection and then insert into DB
                context.Stocks.Add(security);
            }

            //Insert records
            try
            {
                context.SaveChanges();
            }
            catch (Exception exp)
            {
                return OperationStatus.CreateFromException("Error updating security quote.", exp);
            }
            return new OperationStatus { Status = true };
        }

        private OperationStatus InsertExchanges(IEnumerable<string> exchanges, StockDashboardContext context)
        {
            //Insert Exchanges
            foreach (var exchange in exchanges)
            {
                context.Exchanges.Add(new Exchange { Title = exchange });
            }
            try
            {
                context.SaveChanges(); //Save exchanges so we can get their IDs
            }
            catch (Exception exp)
            {
                return OperationStatus.CreateFromException("Error updating security exchange.", exp);
            }
            return new OperationStatus { Status = true };
        }

        private OperationStatus DeleteSecurityRecords(StockDashboardContext context)
        {
            var opStatus = new OperationStatus { Status = false };
            try
            {
                //NOTE: Create Store Procedure or Create Extension Method  to replace this routine
                //opStatus.Status = context.DeleteSecuritiesAndExchanges() == 0;
                foreach(var entity in context.Stocks)
                {
                    context.Stocks.Remove(entity);
                }
                    context.SaveChanges();
            }
            catch (Exception ex)
            {
                return OperationStatus.CreateFromException("Error deleting security/exchange data.", ex);
            }
            return new OperationStatus { Status = true };
        }

        

        //private OperationStatus DeleteSecurityRecords(StockDashboardContext context)
        //{
        //    var opStatus = new OperationStatus { Status = false };
        //    try
        //    {
        //        //Store Procedure
        //        opStatus.Status = context.DeleteSecuritiesAndExchanges() == 0;
        //    }
        //    catch (Exception exp)
        //    {
        //        return OperationStatus.CreateFromException("Error deleting security/exchange data.", exp);
        //    }
        //    return new OperationStatus { Status = true };
        //}
    }
}
