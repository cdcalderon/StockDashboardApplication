using StockDashboard.Core.Interfaces;
using StockDashboard.Core.Model;
using StockDashboard.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace StockDashboard.Controllers
{
    public class StocksController : ApiController
    {
        private readonly IStockRepository _stockRepository;
        public StocksController(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public IEnumerable<Stock> Get(bool topThree = false)
        {
            var results = _stockRepository.Get();
            if (topThree == true)
            {
                if (results.Count() > 0)
                {
                    return results.OrderByDescending(t => t.Change).Take(3).ToList();
                }
                return _stockRepository.GetStockTickerQuotesFromProvider()
                         .OrderByDescending(t => t.Change)
                         .Take(3).ToList();
            }
            else
            {
                if (results.Count() > 0)
                {
                    return results.OrderByDescending(t => t.Open)
                                       .Take(25)
                                       .ToList();
                }
                return _stockRepository.GetStockTickerQuotesFromProvider()
                         .OrderByDescending(t => t.Open)
                         .Take(25)
                         .ToList();
            }
        }

    }
}
