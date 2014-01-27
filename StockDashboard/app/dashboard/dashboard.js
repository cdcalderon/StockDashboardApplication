(function () {
    'use strict';
    var controllerId = 'dashboard';
    angular.module('app').controller(controllerId, ['common', 'datacontext', dashboard]);

    function dashboard(common, datacontext) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.news = {
            title: 'News',
            description: 'Centerbase has released version 2014 and it has tons of new features. It is now easier then ever to view and manage your contacts on a day to day basis..'
        };
        vm.messageCount = 0;
        vm.title = 'Dashboard';

        activate();

        function activate() {
            var promises = [getMessageCount()];
            common.activateController(promises, controllerId)
                .then(function () { log('Activated Dashboard View'); });
        }

        function getMessageCount() {
            return datacontext.getMessageCount().then(function (data) {
                return vm.messageCount = data;
            });
        }
    }
})();