angular.module('admanagement')
    .controller('RootController', ['$scope', 'growl', function ($scope, growl) {

        //Alerts
        //Alerts Display
        $scope.addAlert = function (type, alertmessage) {
            var config = {};
            switch (type) {
                case "success":
                    growl.success(alertmessage, config);
                    alertobject = { Type: "success", Message: alertmessage }
                    break;
                case "info":
                    growl.info(alertmessage, config);
                    alertobject = { Type: "info", Message: alertmessage }
                    break;
                case "warning":
                    growl.warning(alertmessage, config);
                    alertobject = { Type: "warning", Message: alertmessage }
                    $scope.addAlertItem(alertobject);
                    break;
                default:
                    growl.error(alertmessage, config);
                    alertobject = { Type: "error", Message: alertmessage }
                    $scope.addAlertItem(alertobject);
            }
        }
        //Alerts Queue
        $scope.alertitems = [];
        $scope.addAlertItem = function (data) {
            $scope.alertitems.splice(0, 0, angular.copy(data));
        };
        $scope.removeAlertItem = function (data) {
            $scope.alertitems.splice($scope.alertitems.indexOf(data), 1);
        };


        //Progress bar
        $scope.max = 100;
        $scope.number = 0;
        $scope.progress = function (number, type) {
            $scope.number = number;
            $scope.progressbartype = type;
        };
    }]);