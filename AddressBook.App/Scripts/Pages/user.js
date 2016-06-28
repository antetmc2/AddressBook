var root = angular.module('RootApp', ['UserApp', 'ContactApp']);

root.config(['$locationProvider', function ($locationProvider) {
    $locationProvider.html5Mode(true);
}]);

var myApp = angular.module('UserApp', ['ngMessages']);

myApp.controller('UserController', function ($scope, $timeout, UserService) {
    $scope.test = function () {
        $scope.user = 'AAA';
    };

    UserService.GetLoggedUser().then(function (d) {
        $scope.user = d.data;
    });

    $scope.logIn = function() {
        var User = {
            Username: $scope.username,
            Password: $scope.password
        }
        UserService.Login(User).success(function (d) {
            if (d.success === false) alert(d.errors);
            else {
                UserService.GetLoggedUser().then(function (d) {
                    $scope.user = d.data;
                    $scope.username = '';
                    $scope.password = '';
                    //alert('You have successfully logged in!');


                });
            }
        });
    };

    $scope.logOff = function () {
        UserService.LogOff().success(function (d) {
            UserService.GetLoggedUser().then(function (d) {
                $scope.user = d.data;
                //alert('You have successfully logged out!');


            });
        });
    };

    $scope.register = function () {
        var User = {
            Username: $scope.username,
            Password: $scope.password,
            ConfirmPassword: $scope.passwordConfirm,
            Email: $scope.email
        }

        UserService.Register(User).success(function (d) {
            if (d.success === false) alert(d.errors);
            else {
                UserService.GetLoggedUser().then(function (d) {
                    $scope.user = d.data;
                    $scope.username = '';
                    $scope.password = '';
                    $scope.passwordConfirm = '';
                    $scope.email = '';
                    //alert('You have successfully registered your new username!');
                });
            }
        });
    };
});

myApp.factory('UserService', function ($http) {
    var UserService = {};

    UserService.Login = function (user) {
        var response = $http({
            method: "post",
            url: "http://localhost:35949/Data/Login",
            data: JSON.stringify(user),
            dataType: "json"
        });
        return response;
    };

    UserService.Register = function (user) {
        var response = $http({
            method: "post",
            url: "http://localhost:35949/Data/Register",
            data: JSON.stringify(user),
            dataType: "json"
        });
        return response;
    };

    UserService.LogOff = function (user) {
        var response = $http({
            method: "post",
            url: "http://localhost:35949/Data/LogOff",
            dataType: "json"
        });
        return response;
    };

    UserService.GetLoggedUser = function () {
        return $http.get('http://localhost:35949/Data/CurrentUser')
    };

    UserService.RandomUser = function (user) {
        var response = $http({
            method: "post",
            url: "http://localhost:35949/Data/RandomUser",
            dataType: "json"
        });
        return response;
    };

    return UserService;
});

