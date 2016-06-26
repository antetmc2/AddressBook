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

    $scope.addInfo = function (type, info) {
        if (type == 1) var pom = $scope.numbers.pop(info);
        else if (type == 2) var pom = $scope.emails.pop(info);
        else if (type == 3) var pom = $scope.tags.pop(info);
        $scope.hgt = 'automatic';
        ContactService.AddInfo(type, 0, $scope.ID, pom.value).then(function (d) {
            $timeout(function () {
                ContactService.GetContacts('', 1, 'http://localhost:35949/Data/Index').then(function (d) {
                    $scope.Contacts = d.data;

                    ContactService.GetContactById($scope.ID).then(function (d) {
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
                    }, function (error) {
                        alert('Error!');
                    })

                }, function (error) {
                    alert('Error!');
                });
            }, 35);
        }, function (error) {
            alert('Error!');
        });
    };

    $scope.updateInfo = function (type, idinfo, info) {
        ContactService.AddInfo(type, idinfo, $scope.ID, info).then(function (d) {
            $timeout(function () {
                ContactService.GetContacts('', 1, 'http://localhost:35949/Data/Index').then(function (d) {
                    $scope.Contacts = d.data;

                    ContactService.GetContactById($scope.ID).then(function (d) {
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
                    }, function (error) {
                        alert('Error!');
                    })

                }, function (error) {
                    alert('Error!');
                });
            }, 35);
        }, function (error) {
            alert('Error!');
        });
    };

    $scope.save = function (Id) {
        //angular.forEach($scope.numbers, function (state) {
        //    if(state.value != '') $scope.Numbers.push(state.value);
        //});
        //angular.forEach($scope.emails, function (state) {
        //    if (state.value != '') $scope.Emails.push(state.value);
        //});
        //angular.forEach($scope.tags, function (state) {
        //    if (state.value != '') $scope.Tags.push(state.value);
        //});
        var Kontakt = {
            ID: $scope.ID,
            FirstName: $scope.FirstName,
            LastName: $scope.LastName,
            Address: $scope.Address,
            City: $scope.City,
            OIB: $scope.OIB,
            Numbers: $scope.Numbers,
            Emails: $scope.Emails,
            Tags: $scope.Tags
        };

        ContactService.Update(Kontakt);

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


    $scope.remove = function (type, idtype, info) {
        var pom = $scope.ID;
        ContactService.Remove(type, $scope.ID, idtype, info).then(function (d) {
            $timeout(function () {
                $scope.hgt = 'automatic';
                ContactService.GetContacts('', 1, 'http://localhost:35949/Data/Index').then(function (d) {
                    $scope.Contacts = d.data;

                    ContactService.GetContactById(pom).then(function (d) {
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
                    }, function (error) {
                        alert('Error!');
                    })

                }, function (error) {
                    alert('Error!');
                });
            }, 35);
        }, function (error) {
            alert('Error!');
        });
    };

    $scope.delete = function (Id) {
        ContactService.Delete(Id).then(function (d) {
            $timeout(function () {
                ContactService.GetContacts('', 1, 'http://localhost:35949/Data/Index').then(function (d) {
                    $scope.Contacts = d.data;
                }, function (error) {
                    alert('Error!');
                })
            }, 30);
            alert('Uspješno izbrisano!');
        }, function (error) {
            alert('Error!');
        });
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

    ContactService.Update = function (kont) {
        var response = $http({
            method: "post",
            url: "http://localhost:35949/Data/Update",
            data: JSON.stringify(kont),
            dataType: "json"
        });
        return response;
    };

    ContactService.AddInfo = function (type, idinfo, id, info) {
        console.log('A');
        var response = $http({
            method: "post",
            url: "http://localhost:35949/Data/AddUpdateEmailNumber",
            params: { ID: id, IDinfo: idinfo, text: info, type: type }

        });
        return response;
    };

    ContactService.Delete = function (Id) {
        var response = $http({
            method: "post",
            url: "http://localhost:35949/Data/Delete",
            params: { id: Id }
        });
        return response;
    };

    ContactService.Remove = function (type, id, idtype, info) {
        if (type == 'email' || type == 'number') {
            var response = $http({
                method: "post",
                url: "http://localhost:35949/Data/RemoveNumberEmail",
                params: { id: idtype }
            })
            return response;
        }
        else if (type == 'tag') {
            var response = $http({
                method: "post",
                url: "http://localhost:35949/Data/RemoveTag",
                params: { idUser: id, chosenTag: info }
            })
            return response;
        }
    }

    return ContactService;
});