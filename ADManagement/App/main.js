var app = angular.module('admanagement', ['ngRoute', 'ngAnimate', 'ui.bootstrap', 'angular-growl', 'angularUtils.directives.dirPagination', 'ngImgCrop']);

app.config(['$routeProvider', 'growlProvider',
  function ($routeProvider, growlProvider) {
      $routeProvider.
        when('/Users', {
            templateUrl: 'app/views/users.html',
            controller: 'UsersController'
        }).
        when('/Groups', {
            templateUrl: 'app/views/groups.html',
            controller: 'GroupsController'
        }).
        otherwise({
            redirectTo: '/Users'
        });

      growlProvider.globalTimeToLive({ success: 2000, error: 7000, warning: 3000, info: 1000 });
      growlProvider.globalPosition('top-right');
      growlProvider.globalDisableCountDown(false);
  }]);

app.directive('ngEnter', function () {
    return function (scope, element, attrs) {
        element.bind("keydown keypress", function (event) {
            if (event.which === 13) {
                scope.$apply(function () {
                    scope.$eval(attrs.ngEnter);
                });

                event.preventDefault();
            }
        });
    };
});