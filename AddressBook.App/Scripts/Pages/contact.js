var app = angular.module('ContactApp', []);

app.controller('ContactController', function ($scope, $timeout, $compile, $location, ContactService) {
    $scope.Contacts = null;

    $scope.test = function () {
        alert('AAAA');
    };

    console.log('fgggfg');

    ContactService.GetContacts('', 1, 'http://localhost:35949/Data/Index').then(function (d) {
        $scope.Contacts = d.data;
        $scope.term = '';
        $scope.SearchCriteria = 1;
    }, function (error) {
        alert('Error!');
    });

    $scope.add = function () {
        while (angular.element(document.querySelector('.dodano')).length > 0) {
            angular.element(document.querySelector('.dodano')).remove();
        }
        console.log('A');
        $scope.ID = '';
        $scope.FirstName = '';
        $scope.LastName = '';
        $scope.Address = '';
        $scope.City = '';
        $scope.OIB = '';
        $scope.Numbers = [];
        $scope.Emails = [];
        $scope.Tags = [];
        $scope.numbers = [];
        $scope.emails = [];
        $scope.tags = [];
    };

    $scope.create = function () {
        angular.forEach($scope.numbers, function (state) {
            if (state.value != '') $scope.Numbers.push({ ID: 0, PhoneNumber: state.value });
        });
        angular.forEach($scope.emails, function (state) {
            if (state.value != '') $scope.Emails.push({ ID: 0, EmailAddress: state.value });
        });
        angular.forEach($scope.tags, function (state) {
            if (state.value != '') $scope.Tags.push(state.value);
        });
        var Kontakt = {
            FirstName: $scope.FirstName,
            LastName: $scope.LastName,
            Address: $scope.Address,
            City: $scope.City,
            OIB: $scope.OIB,
            Numbers: $scope.Numbers,
            Emails: $scope.Emails,
            Tags: $scope.Tags
        };

        ContactService.Create(Kontakt);

        $timeout(function () {
            ContactService.GetContacts('', 1, 'http://localhost:35949/Data/Index').then(function (d) {
                $scope.Contacts = d.data;
                $scope.term = '';
                $scope.SearchCriteria = 1;
            }, function (error) {
                alert('Error!');
            });
        }, 400);
    };

    $scope.get = function (Id) {
        while (angular.element(document.querySelector('.dodano')).length > 0) {
            angular.element(document.querySelector('.dodano')).remove();
        }
        ContactService.GetContactById(Id).then(function (d) {
            var SelectedContact = d.data;
            $scope.ID = SelectedContact.ID;
            $scope.FirstName = SelectedContact.FirstName;
            $scope.LastName = SelectedContact.LastName;
            $scope.Address = SelectedContact.Address;
            $scope.City = SelectedContact.City;
            $scope.OIB = SelectedContact.OIB;
            $scope.Numbers = SelectedContact.Numbers;
            $scope.Emails = SelectedContact.Emails;
            $scope.Tags = SelectedContact.Tags;
            $scope.numbers = [];
            $scope.emails = [];
            $scope.tags = [];
        }, function (error) {
            alert('Error!');
        })
    };

    var value = 0;
    $scope.numbers = [];
    $scope.emails = [];
    $scope.tags = [];

    $scope.addNum = function () {
        $scope.numbers.push({ value: '' });
        console.log($scope.numbers);
    }

    $scope.addEmail = function () {
        $scope.emails.push({ value: '' });
        console.log($scope.emails);
    }

    $scope.addTag = function () {
        $scope.tags.push({ value: '' });
        console.log($scope.tags);
    }
});

app.factory('ContactService', function ($http) {
    var ContactService = {};

    ContactService.GetContacts = function (Term, Criteria, Url) {
        if (Term == '') return $http.get(Url);
        else {
            return $http.get(Url, {
                params: {
                    term: Term,
                    criteria: Criteria
                }
            });
        }
    };

    ContactService.GetContactById = function (Id) {
        return $http.get('http://localhost:35949/Data/GetContactById', {
            params: {
                id: Id
            }
        });
    };

    ContactService.Create = function (kont) {
        var response = $http({
            method: "post",
            url: "http://localhost:35949/Data/Create",
            data: JSON.stringify(kont),
            dataType: "json"
        });
        return response;
    };

    return ContactService;
});