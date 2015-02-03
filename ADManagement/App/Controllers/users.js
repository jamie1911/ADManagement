angular.module('admanagement')
    .controller('UsersController', function ($scope, $http, $timeout, $modal) {
        $scope.userdataloaded = false;
        $scope.usersList = [];
        $scope.userCount = 0;
        $scope.newManager = '';
        $scope.directSearchPerson = '';
        $scope.photoChanged = false;
        $scope.searchentered = false;

        //user search start
        $scope.userSearch = function (inputUserSearch) {
            $('#inputUserSearch').addClass('loadinggif');
            $scope.selectedUser = null;
            $scope.usersList = [];
            $scope.progress(25);
            $scope.searchentered = true;
            $scope.userdataloaded = false;
            
            $http.get('api/users/GetPeople?people=' + inputUserSearch)
                    .success(function (data, status, headers, config) {
                        if (data.length > 0) {
                            $scope.userCount = data.length;
                            for (var i = 0; i < data.length; i++) {
                                var progressadd = 50 + i
                                $scope.progress(progressadd);
                                $scope.usersList.push(data[i]);
                            }
                            $scope.progress(100, 'success');
                            $('#inputUserSearch').removeClass('loadinggif');
                            $scope.addAlert('success', 'Found Employee(s)');
                            $timeout(function () {
                                $scope.progress(0)
                            }, 1000);
                        }
                        else {
                            $scope.progress(100, 'warning');
                            $scope.addAlert('warning', 'No Employee(s) Found');
                            $('#inputUserSearch').removeClass('loadinggif');
                            $timeout(function () {
                                $scope.progress(0)
                            }, 1000);
                        }
                    })
                    .error(function (data, status, headers, config) {
                        $scope.progress(100, 'danger');
                        $scope.addAlert('error', status)
                        $('#inputUserSearch').removeClass('loadinggif');
                        $timeout(function () {
                            $scope.progress(0)
                        }, 1000);
                    });
        };
        //user search stop

        //start hide usergrid on click
        $scope.isUserGridCollapsed = false;
        //stop hide usergrid on click

        //user group details table pagination setup
        $scope.pagination = {
            current: 1
        };

        //start retrieve user from Active Directory service
        $scope.getUser = function (samAccountName) {
            $scope.userdataloaded = false;
            $scope.progress(25);
            $scope.selectedUser = (
                            $http.get('api/users/GetPerson?person=' + samAccountName)
                                .success(function (data, status, headers, config) {
                                    $scope.selectedUser = null;
                                    $scope.selectedUser = data;
                                    $scope.selectedUser.newDirects = [];
                                    $scope.totalGroups = 0;
                                    $scope.totalGroups = data.memberOf.length;
                                    $scope.groupsPerPage = 15;
                                    $scope.currentPage = 1
                                    $scope.progress(100, 'success');
                                    $scope.userdataloaded = true;
                                    $scope.myPhoto = '';
                                    $scope.myCroppedPhoto = '';
                                    $scope.photoChanged = false;
                                    $scope.changeManager = false;
                                    $scope.newManager = '';
                                    $scope.newDirectList = [];
                                    $("#directSearchPerson").val("");
                                    $scope.directSearchPerson = '';
                                    $timeout(function () {
                                        $scope.progress(0)
                                    }, 1000);
                                })
                                .error(function (data, status, headers, config) {
                                    $scope.selectedUser = null;
                                    $scope.addAlert('error', status);
                                    $scope.progress(100, 'danger');
                                    $scope.userdataloaded = false;
                                    $timeout(function () {
                                        $scope.progress(0)
                                    }, 1000);
                                }));
        };
        //stop retrieve user from Active Directory service

        //start update user
        $scope.updateUser = function (user) {

            var r = confirm("Are you sure you wish to update?");
            if (r == true) {
                $scope.progress(20);
                $http.post('/api/users/SetUserInfo?userInfo=' + user.samAccountName, {
                    'SAMAccountName': user.samAccountName,
                    'Title': user.title,
                    'Department': user.department,
                    'DistinguishedName': user.distinguishedName,
                    'Telephone': user.telephone,
                    'ManagerDistinguishedName': user.managerDistinguishedName,
                    'Office': user.office,
                    'ThumbnailPhoto': user.thumbnailPhoto,
                    'NewDirects': user.newDirects
                })
                
                .success(function (data, status, headers, config) {
                    $scope.progress(100, 'success');
                    $scope.addAlert(data.type, data.message);
                    if (data.statusDetail.length > 0) {
                        for (index = 0; index < data.statusDetail.length; ++index) {
                            $scope.addAlert(data.statusDetail[index].statusDescType, data.statusDetail[index].statusDesc)
                        }
                    }
                    $scope.getUser(user.samAccountName);
                    $timeout(function () {
                        $scope.progress(0)
                    }, 1000);
                })
                .error(function (data, status, headers, config) {
                    $scope.progress(100, 'danger');
                    $scope.addAlert('error', status);
                    $timeout(function () {
                        $scope.progress(0)
                    }, 1000);
                });
            } else {
            }
        };
        //stop update user

        //start photo editing
        $scope.showPhotoModal = function () {
            var PhotoModal = $modal.open({
                templateUrl: 'App/Templates/Modals/ChangePhoto.html',
                size: "lg",
                scope: $scope,
                backdrop: true,
                windowClass: "modal"
            })

            $scope.savePhotoModal = function () {
                $scope.selectedUser.thumbnailPhoto = this.myCroppedPhoto;
                PhotoModal.dismiss();
            };

            $scope.closePhotoModal = function () {
                PhotoModal.dismiss();
            };
        };
        //handle file upload/select
        $scope.handlePhotoFileSelect = function (evt) {
            var file = evt.files[0];
            var reader = new FileReader();
            reader.onload = function (evt) {
                $scope.$apply(function ($scope) {
                    $scope.myPhoto = evt.target.result;
                    $scope.photoChanged = true;
                });
            };

            reader.readAsDataURL(file);
        }
        //stop photo editing

        //start new directreport
        $scope.newDirectList = [];
        $scope.disableAddDirect = true;
        $scope.addNewDirectReport = function (data) {
            $scope.selectedUser.newDirects.splice(0, 0, angular.copy(data.samAccountName));
            $scope.newDirectList.splice(0, 0, angular.copy(data));
            $scope.disableAddDirect = true;
            $("#directSearchPerson").val("");
        }
        $scope.directSearch = function (viewValue) {
            $('#directSearchPerson').addClass('loadinggif');
            return $http.get('api/users/GetPeople?people=' + viewValue)
                .then(function (response) {
                    $('#directSearchPerson').removeClass('loadinggif');
                   if (response.data < 1) {
                       $scope.addAlert('warning', 'Please refine search for new direct report');
                        return response.data
                    }
                    else {
                        return response.data
                    }
                })
        };
        $scope.removeDirect = function (person) {
            $scope.newDirectList.splice($scope.newDirectList.indexOf(person), 1);
            $scope.selectedUser.newDirects.splice($scope.selectedUser.newDirects.indexOf(person.samAccountName), 1);
        };
        //stop new directreport

        //start change manager
        $scope.changeTheManger = function () {
            $scope.changeManager = true;
            $scope.disabledAddManager = false;
        }
        $scope.managerSearch = function (viewValue) {
            $('#newManager').addClass('loadinggif');
            return $http.get('api/users/GetPeople?people=' + viewValue)
                .then(function (response) {
                    $('#newManager').removeClass('loadinggif');
                    if (response.data < 1) {
                        $scope.addAlert('warning', 'Please refine search for new manager');
                        return response.data;
                    }
                    else {
                        return response.data
                    }
                })
        };
        $scope.addNewManager = function (data) {
            $scope.changeManager = false;
            $scope.disabledAddManager = true;
            $scope.selectedUser.managerDistinguishedName = data.distinguishedName;
        }
        //stop change manager

        var init = function () {

        }

        init();
    });