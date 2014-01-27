(function () {
    'use strict';
    // Define the common module 
    var commonModule = angular.module('common', []);
    commonModule.provider('commonConfig', function () {
        this.config = {
        };

        this.$get = function () {
            return {
                config: this.config
            };
        };
    });

    commonModule.factory('common',
        ['$q', '$rootScope', '$timeout', '$http', 'commonConfig', 'logger', common]);

    function common($q, $rootScope, $timeout, $http, commonConfig, logger) {
        var throttles = {};

        var service = {
            // common angular dependencies
            $broadcast: $broadcast,
            $q: $q,
            $http: $http,
            $timeout: $timeout,
            // generic
            activateController: activateController,
            logger: logger, // for accessibility
        };

        return service;
        function activateController(promises, controllerId) {
            return $q.all(promises).then(function (eventArgs) {
                var data = { controllerId: controllerId };
                $broadcast(commonConfig.config.controllerActivateSuccessEvent, data);
            });
        }

        function $broadcast() {
            return $rootScope.$broadcast.apply($rootScope, arguments);
        }
    }
})();