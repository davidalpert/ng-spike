﻿var app = angular.module('app', ['ngRoute'])
    .config(function ($routeProvider, $locationProvider) {
        $routeProvider.
            when('/about', {
                templateUrl: 'js/views/about.html',
            }).
            when('/teams/:id', {
                templateUrl: 'js/views/teamsDetail.html',
                controller: 'teamsDetailController',
            }).
            when('/games/:id', {
                templateUrl: 'js/views/gamesDetail.html',
                controller: 'gamesDetailController',
            }).
            when('/', {
                templateUrl: 'js/views/games.html',
                controller: 'gamesController',
            });
        
        $routeProvider.otherwise({ redirectTo: '/' });

        //$locationProvider.html5Mode(true);
    });

app.controller('navController', function ($scope, $location) {
    $scope.isActive = function (viewLocation) {
        return viewLocation === $location.path();
    };
});

app.controller('teamsDetailController', function ($scope, $http, $routeParams) {
    $http.get('/api/teams/' + $routeParams.id).
        success(function(data, status) {
            //console.log('status: ' + status);
            $scope.status = status;
            $scope.team = data;
        }).
        error(function(data, status, headers, config) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
            //console.log('status: ' + status);
            $scope.status = status;
            $scope.errorMessage = data.Message;
            $scope.errorDetail = data.MessageDetail;
    });
});

app.controller('gamesDetailController', function ($scope, $http, $routeParams) {
    $http.get('/api/games/' + $routeParams.id).
        success(function (data, status) {
            //console.log('status: ' + status);
            $scope.status = status;
            $scope.game = data;
        }).
        error(function(data, status, headers, config) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
            //console.log('status: ' + status);
            $scope.status = status;
            $scope.errorMessage = data.Message;
            $scope.errorDetail = data.MessageDetail;
    });
});

// date formatting courtesy of: http://blog.stevenlevithan.com/archives/date-time-format
app.controller('gamesController', function ($scope, $http) { //, $route, $routeParams, $location, $http) {
    $http.get('/api/games').
        success(function(data, status) {
            //console.log('status: ' + status);
            $scope.status = status;
            $scope.games = data;
            angular.forEach($scope.games, function (g, i) {
                g.DatePlayed = new Date(g.DatePlayed).format('mediumDate');
            });
        }).
        error(function(data, status, headers, config) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
            //console.log('status: ' + status);
            $scope.status = status;
            $scope.errorMessage = data.Message;
            $scope.errorDetail = data.MessageDetail;
    });
});
