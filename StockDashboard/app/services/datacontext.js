(function () {
    'use strict';

    var serviceId = 'datacontext';
    angular.module('app').factory(serviceId,
        ['common', datacontext]);

    function datacontext(common) {
        var $q = common.$q;
        var $http = common.$http;
        var service = {
            getMessageCount: getMessageCount,
            getStockQuotes: getStockQuotes,
            getTopThreeStockQuotes: getTopThreeStockQuotes
        };

        return service;

        function getMessageCount() { return $q.when(24); }

        function getStockQuotes() {
            return $http.get("/api/stocks");
        }

        function getTopThreeStockQuotes() {
            return $http.get("/api/stocks?topThree=true");
        }

        function getSampleTestStockQuotesData() {
            //var stockQuotes = [
            //    { Symbol: 'IBM',  Last: 250 },
            //    { Symbol: 'MSFT', Last: 35  },
            //    { Symbol: 'CSCO', Last: 26 },
            //    { Symbol: 'AAPL', Last: 567 }
            //];
            //return $q.when(stockQuotes);
        }
    }
})();