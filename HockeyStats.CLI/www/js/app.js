
var app = angular.module('app', ['ngRoute'])
    .config(function ($routeProvider, $locationProvider) {
        console.log($routeProvider);
        
        $routeProvider.when('/', {
            templateUrl: 'js/views/games.html',
            controller: 'gamesController',
        });

        //$routeProvider.otherwise({ redirectTo: '/' });

        //$locationProvider.html5Mode(true);
        console.log('finished.');
    });

app.controller('gamesController', function ($scope, $http) { //, $route, $routeParams, $location, $http) {
    $http.get('/api/games').
        success(function (data, status) {
            $scope.status = status;
            $scope.games = data;
            console.log('status: ' + status);
        }).
        error(function(data, status, headers, config) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
            $scope.status = status;
            $scope.errorMessage = data.Message;
            $scope.errorDetail = data.MessageDetail;
            console.log('status: ' + status);
    });
});
