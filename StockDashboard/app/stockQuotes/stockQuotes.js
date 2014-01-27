(function () {
    'use strict';

    // Controller name is handy for logging
    var controllerId = 'stockQuotes';

    // Define the controller on the module.
    // Inject the dependencies. 
    // Point to the controller definition function.
    angular.module('app').controller(controllerId,
        ['common', 'datacontext', stockQuotes]);

    function stockQuotes(common, datacontext) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        // Using 'Controller As' syntax, so we assign this to the vm variable (for viewmodel).
        var vm = this;
        // Bindable properties and functions are placed on vm.
        vm.title = 'StockQuotes';
        vm.stockQuotes = [];
        vm.topThreeStockQuotes = [];

        activate();
        function activate() {
            //this takes care of set the spinner off when data es fetched successfully
            common.activateController([getStockQuotes(), getTopThreeStockQuotes()], controllerId)
                .then(function() {log('Stock Quotes Fetched Successfully')})
        }

        function getStockQuotes() {
            datacontext.getStockQuotes().then(function (result) {
                        angular.copy(result.data, vm.stockQuotes);
                    }, function () {
                        alert("error");
                    });
        }

        function getTopThreeStockQuotes() {
            datacontext.getTopThreeStockQuotes().then(function (result) {
                angular.copy(result.data, vm.topThreeStockQuotes);
            }, function () {
                alert("error");
            });
        }


        //#region Internal Methods 
        //#endregion
    }
})();
