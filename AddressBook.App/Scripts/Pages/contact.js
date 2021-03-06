﻿var app = angular.module('ContactApp', ['UserApp']);

app.run(function ($rootScope, $location, ContactService) {
    $rootScope.$on('$locationChangeSuccess', function () {
        $rootScope.actualLocation = $location.search();
    });

    $rootScope.$watch(function () { return $location.search() }, function (newLocation, oldLocation) {
        if ($rootScope.actualLocation === newLocation) {
            var scope = angular.element(document.getElementById('glavni')).scope();
            scope.searchButton();
        }
    });
});

app.controller('ContactController', function ($scope, $timeout, $compile, $location, ContactService) {
    $scope.Contacts = null;
    $scope.CurrUser = null;
    $scope.TagsList = null;
    $scope.test = function () {
        alert('AAAA');
    };

    ContactService.GetContacts('', 1, '/Data/Index').then(function (d) {
        $scope.Contacts = d.data;
        $scope.term = '';
        $scope.SearchCriteria = 1;
        $scope.SelectedTag = 0;
    }, function (error) {
        alert('Error!');
    });

    ContactService.GetTags().then(function (d) {
        $scope.TagsList = d.data;
    }, function (error) {
        alert('Error!');
    });

    $scope.$watch(function () { return document.getElementById("loggedUser").innerHTML },
        function (newVal, oldVal) {
            $scope.CurrUser = newVal;
            if ($scope.CurrUser == '') $scope.message = 'Please log in or register to use the application!';
            else $scope.message = '';
        });

    $scope.search = function () {
        ContactService.SearchContacts($scope.term, $scope.SearchCriteria).then(function (d) {
            $scope.Contacts = d.data;
            var path = $location.path(); //Path without parameters, e.g. /search (without ?q=test)
            $location.url(path + '?term=' + $scope.term + '&criteria=' + $scope.SearchCriteria);
        }, function (error) {
            alert('Error!');
        })
    };

    $scope.searchButton = function () {
        var params = $location.search();
        console.log(params);
        console.log(params.term);
        if (params.filter != undefined)
        {
            ContactService.Filter(params.filter).then(function (d) {
                $scope.Contacts = d.data;
            }, function (error) {
                alert('Error!');
            })
        }

        if (params.term != undefined) {
            $scope.term = params.term;
            ContactService.SearchContacts(params.term, params.criteria).then(function (d) {
                $scope.Contacts = d.data;
            }, function (error) {
                alert('Error!');
            })
        }

        if (params.filter == undefined && params.term == undefined && params.criteria == undefined) {
            $scope.term = '';
            $scope.SearchCriteria = 1;
            $scope.SelectedTag = 0;
            ContactService.SearchContacts($scope.term, $scope.SearchCriteria).then(function (d) {
                $scope.Contacts = d.data;
                var path = $location.path();
                $location.url(path);
            }, function (error) {
                alert('Error!');
            })
        }
    };

    $scope.reset = function () {
        $scope.term = '';
        $scope.SearchCriteria = 1;
        $scope.SelectedTag = 0;
        ContactService.SearchContacts($scope.term, $scope.SearchCriteria).then(function (d) {
            $scope.Contacts = d.data;
            var path = $location.path();
            $location.url(path);
        }, function (error) {
            alert('Error!');
        })
    };

    $scope.filter = function ()
    {
        ContactService.Filter($scope.SelectedTag).then(function (d) {
            $scope.Contacts = d.data;
            $scope.term = '';
            $scope.SearchCriteria = 1;
            var path = $location.path(); //Path without parameters, e.g. /search (without ?q=test)
            $location.url(path + '?filter=' + $scope.SelectedTag);
        }, function (error) {
            alert('Error!');
        })
    };

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

        ContactService.Create(Kontakt).then(function () {
            $timeout(function () {
                ContactService.GetContacts('', 1, '/Data/Index').then(function (d) {
                    $scope.Contacts = d.data;
                    $scope.term = '';
                    $scope.SearchCriteria = 1;
                }, function (error) {
                    alert('Error!');
                });
            }, 400);
            ContactService.GetTags().then(function (d) {
                $scope.TagsList = d.data;
            }, function (error) {
                alert('Error!');
            });
        });

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
                ContactService.GetContacts('', 1, '/Data/Index').then(function (d) {
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

                    ContactService.GetTags().then(function (d) {
                        $scope.TagsList = d.data;
                    }, function (error) {
                        alert('Error!');
                    });

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
                ContactService.GetContacts('', 1, '/Data/Index').then(function (d) {
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
            ContactService.GetContacts('', 1, '/Data/Index').then(function (d) {
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
                ContactService.GetContacts('', 1, '/Data/Index').then(function (d) {
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
                ContactService.GetContacts('', 1, '/Data/Index').then(function (d) {
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

app.factory('ContactService', function ($http, UserService) {
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

    ContactService.SearchContacts = function (Term, Criteria) {
        if (Term == '') return $http.get('/Data/Index');
        if (Criteria == 1) {
            return $http.get('/Data/SearchByFirstName', {
                params: {
                    term: Term
                }
            });
        }
        else if (Criteria == 2) {
            return $http.get('/Data/SearchByLastName', {
                params: {
                    term: Term
                }
            });
        }
        else if (Criteria == 3) {
            return $http.get('/Data/SearchByTag', {
                params: {
                    term: Term
                }
            });
        }
        else return $http.get('/Data/Index');
    };

    ContactService.Filter = function (Id) {
        return $http.get('/Data/Filter', {
            params: {
                tagID: Id
            }
        });
    };

    ContactService.GetContactById = function (Id) {
        return $http.get('/Data/GetContactById', {
            params: {
                id: Id
            }
        });
    };

    ContactService.Create = function (kont) {
        var response = $http({
            method: "post",
            url: "/Data/Create",
            data: JSON.stringify(kont),
            dataType: "json"
        });
        return response;
    };

    ContactService.Update = function (kont) {
        var response = $http({
            method: "post",
            url: "/Data/Update",
            data: JSON.stringify(kont),
            dataType: "json"
        });
        return response;
    };

    ContactService.AddInfo = function (type, idinfo, id, info) {
        console.log('A');
        var response = $http({
            method: "post",
            url: "/Data/AddUpdateEmailNumber",
            params: { ID: id, IDinfo: idinfo, text: info, type: type }

        });
        return response;
    };

    ContactService.Delete = function (Id) {
        var response = $http({
            method: "post",
            url: "/Data/Delete",
            params: { id: Id }
        });
        return response;
    };

    ContactService.Remove = function (type, id, idtype, info) {
        if (type == 'email' || type == 'number') {
            var response = $http({
                method: "post",
                url: "/Data/RemoveNumberEmail",
                params: { id: idtype }
            })
            return response;
        }
        else if (type == 'tag') {
            var response = $http({
                method: "post",
                url: "/Data/RemoveTag",
                params: { idUser: id, chosenTag: info }
            })
            return response;
        }
    };

    ContactService.GetTags = function () {
        return $http.get('/Data/GetUsedTags');
    };

    return ContactService;
});