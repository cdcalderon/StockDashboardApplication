using StockDashboard.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockDashboard.Data.Utils
{
    public class StockEngine
    {
        //NOTE: Refactor Using the Strategy Pattern to have 2 different Parsers "Yahoo" and "Google"
        //
        private const string BASE_URL = "http://www.google.com/ig/api?";
        private readonly string _Separator = "&stock=";

        //Debug This to see why is not working
        //private const string BASE_URLYahooApi = "http://query.yahooapis.com/v1/public/yql?q=" +
        //                                "select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20({0})" +
        //                                "&amp;env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys";

        private const string BASE_URLYahooApi = "http://query.yahooapis.com/v1/public/yql?q=" +
                                        "select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20({0})" +
                                        "&env=http://datatables.org/alltables.env";

        

        public List<Stock> GetSecurityQuotesFromYahooFinance(IEnumerable<String> quotes)
        {
            
            string symbolList = String.Join("%2C", quotes.Select(s => "%22" + s + "%22").ToArray());
            string url = string.Format(BASE_URLYahooApi, symbolList);

            XDocument doc = XDocument.Load(url);
           var stocks =  ParseSecuritiesFromYahoo(quotes, doc);
           return stocks;
        }

        //Get Security Quotes From Google
        public List<Stock> GetSecurityQuotes(params string[] symbols)
        {
            XDocument doc = CreateXDocument(symbols);
            return ParseSecurities(doc);
        }

        public List<MarketIndex> GetMarketQuotes(string[] symbols)
        {
            XDocument doc = CreateXDocument(symbols);
            return ParseMarketIndexes(doc);
        }

        private XDocument CreateXDocument(string[] symbols)
        {
            string symbolList = String.Join(_Separator, symbols);
            string url = string.Concat(BASE_URL, _Separator, symbolList, "&Ticks=", DateTime.Now.Ticks);

            try
            {
                XDocument doc = XDocument.Load(url);
                return doc;
            }
            catch { }
            return null;
        }

        private List<Stock> ParseSecurities(XDocument doc)
        {
            if (doc == null) return null;
            List<Stock> stocks = new List<Stock>();

            IEnumerable<XElement> quotes = doc.Root.Descendants("finance");

            foreach (var quote in quotes)
            {
                var symbol = GetAttributeData(quote, "symbol");
                var exchange = GetAttributeData(quote, "exchange");
                var last = GetDecimal(quote, "last");
                var change = GetDecimal(quote, "change");
                var percentChange = GetDecimal(quote, "perc_change");
                var company = GetAttributeData(quote, "company");

                var stock = new Stock();
                stock.Symbol = symbol;
                stock.Last = last;
                stock.Change = change;
                stock.PercentChange = percentChange;
                stock.RetrievalDateTime = DateTime.Now;
                stock.Company = company;
                stock.Exchange = new Exchange { Title = exchange };
                stock.DayHigh = GetDecimal(quote, "high");
                stock.DayLow = GetDecimal(quote, "low");
                stock.Volume = GetDecimal(quote, "volume");
                stock.AverageVolume = GetDecimal(quote, "avg_volume");
                stock.MarketCap = GetDecimal(quote, "market_cap");
                stock.Open = GetDecimal(quote, "open");
                stock.ImageSource = symbol + ".jpg";
                stocks.Add(stock);
                
            }
            return stocks;
        }

        private List<MarketIndex> ParseMarketIndexes(XDocument doc)
        {
            if (doc == null) return null;
            List<MarketIndex> marketIndexes = new List<MarketIndex>();

            IEnumerable<XElement> quotes = doc.Root.Descendants("finance");

            foreach (var quote in quotes)
            {
                var index = new MarketIndex();
                index.Symbol = GetAttributeData(quote, "symbol"); ;
                index.Last = GetDecimal(quote, "last");
                index.Change = GetDecimal(quote, "change");
                index.PercentChange = GetDecimal(quote, "perc_change");
                index.RetrievalDateTime = DateTime.Now;
                index.Title = GetAttributeData(quote, "company");
                index.DayHigh = GetDecimal(quote, "high");
                index.DayLow = GetDecimal(quote, "low");
                index.Volume = GetDecimal(quote, "volume");
                index.Open = GetDecimal(quote, "open");
                marketIndexes.Add(index);
            }
            return marketIndexes;
        }

        private string GetAttributeData(XElement quote, string elemName)
        {
            return quote.Element(elemName).Attribute("data").Value;
        }

        private decimal GetDecimal(XElement quote, string elemName)
        {
            var input = GetAttributeData(quote, elemName);
            if (input == null) return 0.00M;

            decimal value;

            if (Decimal.TryParse(input, out value)) return value;
            return 0.00M;
        }

        private long GetLong(XElement quote, string elemName)
        {
            var input = GetAttributeData(quote, elemName);
            if (input == null) return 0L;

            long value;

            if (long.TryParse(input, out value)) return value;
            return 0L;
        }

        private DateTime GetDateTime(XElement quote, string elemName)
        {
            var input = GetAttributeData(quote, elemName);
            if (input == null) return DateTime.Now; ;

            DateTime value;

            if (DateTime.TryParse(input, out value)) return value;
            return DateTime.Now;
        }

        private List<Stock> ParseSecuritiesFromYahoo(IEnumerable<string> quotes, XDocument doc)
        {
            XElement results = doc.Root.Element("results");
            List<Stock> stocks = new List<Stock>();
            foreach (var quote in quotes)
            {
                XElement q = results.Elements("quote").FirstOrDefault(w => w.Attribute("symbol").Value == quote);

                if (q != null)
                {
                    var stock = new Stock();
                    stock.AverageVolume = GetDecimal(q.Element("AverageDailyVolume").Value);
                    stock.Symbol = quote;
                    stock.Change = GetDecimal(q.Element("Change").Value);
                    stock.DayLow = GetDecimal(q.Element("DaysLow").Value);
                    stock.DayHigh = GetDecimal(q.Element("DaysHigh").Value);
                    stock.MarketCap = GetDecimal(q.Element("MarketCapitalization").Value);
                    stock.Last = GetDecimal(q.Element("LastTradePriceOnly").Value);
                    stock.Company = q.Element("Name").Value;
                    stock.Open = GetDecimal(q.Element("Open").Value);
                    stock.PercentChange = GetDecimal(q.Element("ChangeinPercent").Value);
                    stock.Volume = GetDecimal(q.Element("Volume").Value);
                    stock.Exchange = new Exchange { Title = q.Element("StockExchange").Value };
                    stock.ImageSource = quote + ".jpg";
                    stocks.Add(stock);
                }
            }
            return stocks;
        }

        private static decimal GetDecimal(string input)
        {
            if (input == null) return 0;

            input = input.Replace("%", "");

            decimal value;

            if (Decimal.TryParse(input, out value)) return value;
            return 0;
        }

        private static DateTime? GetDateTime(string input)
        {
            if (input == null) return null;

            DateTime value;

            if (DateTime.TryParse(input, out value)) return value;
            return null;
        }
    }
}
