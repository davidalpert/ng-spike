
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
    console.log('gamesController');
    $http.get('/api/games').
        success(function(data) {
            alert(data);
            $scope.games = data;
            $scope.status = 200;
        }).
        error(function(data, status, headers, config) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        $scope.status = status;
    });
});
