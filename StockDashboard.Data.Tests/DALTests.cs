using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StockDashboard.Core.Interfaces;
using StockDashboard.Core.Model;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Moq;

namespace StockDashboard.Data.Tests
{
    [TestClass]
    public class DALTests
    {
        [Ignore]
        [TestMethod]
        public void thisMethodTestRepositoryUsage()
        {
            RepositoryTestClass repositoryTest = new RepositoryTestClass();

            IEnumerable<Stock> stocks = repositoryTest.GetStocks();

            Assert.IsTrue(stocks != null);
        }

        [TestMethod]
        public void repositoryMocking()
        {
            List<Stock> stocks = new List<Stock>()
            {
                new Stock() { Symbol = "CSCO", Last = 16},
                new Stock() { Symbol = "MSFT", Last = 36 }
            };

            Mock<IStockRepository> mockCarRepository = new Mock<IStockRepository>();
            mockCarRepository.Setup(obj => obj.Get()).Returns(stocks);

            RepositoryTestClass repositoryTest = new RepositoryTestClass(mockCarRepository.Object);

            IEnumerable<Stock> ret = repositoryTest.GetStocks();

            Assert.IsTrue(ret == stocks);
        }
    }

    public class RepositoryTestClass
    { 
        public RepositoryTestClass()
        {
            ObjectBase.Container.SatisfyImportsOnce(this);
        }

        [Import]
        IStockRepository _stockRepository;

        public RepositoryTestClass(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public IEnumerable<Stock> GetStocks()
        {
            IEnumerable<Stock> cars = _stockRepository.Get();

            return cars;
        }
    }
}
