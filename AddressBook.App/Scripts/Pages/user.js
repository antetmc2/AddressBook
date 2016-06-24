var myApp = angular.module('UserApp', []);

myApp.controller('UserController', function ($scope) {
    $scope.test = function () {
        alert('AAAAAA');
    };
});

myApp.factory('UserService', function ($http) {
    var UserService = {};

    ContactService.Login = function (user) {
        var response = $http({
            method: "post",
            url: "http://localhost:31276/Data/Login",
            data: JSON.stringify(user),
            dataType: "json"
        });
        return response;
    };

    ContactService.Register = function (user) {
        var response = $http({
            method: "post",
            url: "http://localhost:31276/Data/Register",
            data: JSON.stringify(user),
            dataType: "json"
        });
        return response;
    };

    return UserService;
});