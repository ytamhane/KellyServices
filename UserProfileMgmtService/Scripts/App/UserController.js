/// <reference path="../Scripts/angular.js" />
/// <reference path="MainAppController.js" />
userProfileModule.controller("userController", function ($scope, $http, UserService, $uibModal) {
    var $ctrl = this;
    $scope.users = [];
    $scope.maxSize = 10;
    $scope.currentPage = 1;
    $scope.userId = '';
    $scope.getDeleted = false;
    $scope.selectedUser = [];

    //Get data for page when the page shows up for the first time
    GetDataForPage($scope, $http, UserService, $scope.currentPage, $scope.userId, $scope.getDeleted);

    //Page change event handled
    $scope.pageChanged = function () {
        GetDataForPage($scope, $http, UserService, $scope.currentPage, $scope.userId, $scope.getDeleted);

    };

    //Search a user by userid
    $scope.SearchUserId = function (userid) {
        $scope.userId = userid;
        GetDataForPage($scope, $http, UserService, $scope.currentPage, $scope.userId, $scope.getDeleted);
    };

    $scope.DeleteUsers = function (index) {
        UserService.DeleteProfiles(index, $scope);
    };

    //Modify button and screen handled
    $ctrl.modify = function (size, user) {
        $ctrl.selectedUser = user;
        var parentElem = undefined; //parentSelector ? angular.element($document[0].querySelector('.modal-demo ' + parentSelector)) : undefined;
        var modalInstance = $uibModal.open({
            animation: $ctrl.animationsEnabled,
            ariaLabelledBy: 'modify-user-modal-title',
            ariaDescribedBy: 'modify-user-modal-body',
            templateUrl: 'ModifyUserModal.html',
            controller: 'ModifyUserInstanceCtrl',
            controllerAs: '$ctrl',
            size: size,
            appendTo: parentElem,
            resolve: {
                selectedUserToModify: function () {
                    return $ctrl.selectedUser;
                }
            }
        });

        modalInstance.result.then(function () {
            location.reload();
        }, function () {
        });
    };

    ///Add new user button and screen handled
    $ctrl.addNew = function (size) {
        var parentElem = undefined;
        var modalAddInstance = $uibModal.open({
            animation: $ctrl.animationsEnabled,
            ariaLabelledBy: 'add-user-modal-title',
            ariaDescribedBy: 'add-user-modal-body',
            templateUrl: 'AddUserModal.html',
            controller: 'AddUserInstanceCtrl',
            controllerAs: '$ctrl',
            size: size,
            appendTo: parentElem
        });

        modalAddInstance.result.then(function () {
            location.reload();
        }, function () {
        });
    };


    //assign profiles
    $ctrl.assignProfiles = function (size, user) {
        $ctrl.selectedUser = user;
        var parentElem = undefined; //parentSelector ? angular.element($document[0].querySelector('.modal-demo ' + parentSelector)) : undefined;
        var modalInstance = $uibModal.open({
            animation: $ctrl.animationsEnabled,
            ariaLabelledBy: 'assign-profile-modal-title',
            ariaDescribedBy: 'assign-profile-modal-body',
            templateUrl: 'AssignProfilesModal.html',
            controller: 'AssignProfilesInstanceCtrl',
            controllerAs: '$ctrl',
            size: size,
            appendTo: parentElem,
            resolve: {
                selectedUserToModify: function () {
                    return $ctrl.selectedUser;
                }
            }
        });

        modalInstance.result.then(function () {
            $scope.assignProfilesToUser = false;
            location.reload();
        }, function () {
        });
    };


});


//controller for modify user 
angular.module('userProfileModule').controller('ModifyUserInstanceCtrl', function ($uibModalInstance, selectedUserToModify, UserService) {
    var $ctrl = this;
    $ctrl.selectedUserToModify = {
        uID: selectedUserToModify.uID,
        UserID: selectedUserToModify.UserID,
        UserName: selectedUserToModify.UserName,
        UpdatedBy: selectedUserToModify.UpdatedBy,
        UpdatedOn: selectedUserToModify.UpdatedOn
    };

    $ctrl.validateMessage = '';
    $ctrl.isModifyUserDisabled = false;

    $ctrl.ok = function () {
        validateMessage = Validate($ctrl.selectedUserToModify);
        if (validateMessage == '') {
            UserService.UpdateUser($ctrl.selectedUserToModify, $uibModalInstance, $ctrl);
        }
        else {
            $ctrl.isModifyUserDisabled = false;
            alert(validateMessage);
        }
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});

//controller for add profile 
angular.module('userProfileModule').controller('AddUserInstanceCtrl', function ($uibModalInstance, UserService) {
    var $ctrl = this;
    $ctrl.newUser = {
        uID: 0,
        UserID: '',
        UserName: ''
    };
    $ctrl.isAddUserDisabled = false;
    $ctrl.validateMessage = '';

    $ctrl.AddNewUser = function () {
        validateMessage = Validate($ctrl.newUser);
        if (validateMessage == '') {
            UserService.AddUser($ctrl.newUser, $uibModalInstance, $ctrl);
        }
        else {
            $ctrl.isAddUserDisabled = false;
            alert(validateMessage);
        }
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});

//controller for assign profiles 
angular.module('userProfileModule').controller('AssignProfilesInstanceCtrl', function ($uibModalInstance, selectedUserToModify, UserService) {
    var $ctrl = this;
    $ctrl.selectedUserToModify = {
        uID: selectedUserToModify.uID,
        UserID: selectedUserToModify.UserID,
        UserName: selectedUserToModify.UserName,
        UserProfileEntities: selectedUserToModify.UserProfileEntities
    };
    $ctrl.isAssignProfilesDisabled = false;

    $ctrl.AllProfiles = [];
    $ctrl.errorMessageProfile = '';
    UserService.GetAllProfilesToAssign()
    .then(
        function (response) {
            $ctrl.AllProfiles = response.data.profileEntities;
            $ctrl.errorMessageProfile = '';
        },
        function (error) {
            $ctrl.AllProfiles = [];
            $ctrl.errorMessageProfile = 'No profiles found';
        }
    );
    $ctrl.selectedProfilesToAssign = [];
    for (var i = 0; i < $ctrl.selectedUserToModify.UserProfileEntities.length; i++) {
        $ctrl.selectedProfilesToAssign.push($ctrl.selectedUserToModify.UserProfileEntities[i].pID);
    }
    $ctrl.toggleProfileSelection = function toggleProfileSelection(pID) {
        var idx = $ctrl.selectedProfilesToAssign.indexOf(pID);
        if (idx > -1) {
            $ctrl.selectedProfilesToAssign.splice(idx, 1);
        }
        else {
            $ctrl.selectedProfilesToAssign.push(pID);
        }
    };

    $ctrl.assignProfilesToUser = function () {
        UserService.AssignProfiles($ctrl.selectedUserToModify.uID, $ctrl.selectedProfilesToAssign, $uibModalInstance, $ctrl);
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };



});


//Validate profile id and description on Modify or Add New 
function Validate(selectedUser) {
    var validateMessage = '';
    if (selectedUser.UserID.trim() === '') {
        validateMessage += 'User ID cannot be blank. \n';
    }

    var splChars = " *|,\":<>[]{}`\';()@&$#%";
    for (var i = 0; i < selectedUser.UserID.length; i++) {
        if (splChars.indexOf(selectedUser.UserID.charAt(i)) != -1) {
            validateMessage += 'User Id cannot have special characters. \n';
            break;
        }
    }

    if (selectedUser.UserName.trim() === '') {
        validateMessage += 'User Name cannot be blank. \n';
    }

    //var splChars = "*|,\":<>[]{}`\';()@&$#%";
    //for (var i = 0; i < selectedUser.UserName.length; i++) {
    //    if (splChars.indexOf(selectedUser.UserName.charAt(i)) != -1) {
    //        validateMessage += 'User Name cannot have special characters. \n';
    //        break;
    //    }
    //}
    return validateMessage;
}

function GetDataForPage($scope, $http, UserService, pageNumber, userId, getDeleted) {
    UserService.GetAllUsers(pageNumber, userId, getDeleted)
			.then(
				function (response) {
				    $scope.users = response.data.userEntities;
				    $scope.totalItems = response.data.userCount;
				    $scope.currentPage = pageNumber;
				    $scope.errorMessage = '';
				    $scope.numberOfRowsPerPage = response.data.numberOfRowsPerPage;
				},
                function (error) {
                    $scope.errorMessage = "No Data Found";
                    $scope.users = [];
                    $scope.totalItems = 0;
                    $scope.currentPage = 1;
                    $scope.numberOfRowsPerPage = 0;
                });

}

userProfileModule.factory('UserService', function ($http) {
    var usrFac = {};
    usrFac.GetAllUsers = function (pageNumber, userId, getDeleted) {
        var urlToGet = '../api/User?pageNumber=' + pageNumber + "&userId=" + userId + "&getDeleted=" + getDeleted;
        return $http.get(urlToGet);
    };

    usrFac.UpdateUser = function (updatedUser, $uibModalInstance, $ctrl) {
        var urlToUpdate = '../api/User';
        $http({
            method: 'PUT',
            url: urlToUpdate,
            data: updatedUser
        }).then(function successCallback(response) {
            if (response.data == '') {

                alert("User Updated Successfully !!!");
                $uibModalInstance.close();
            }
            else {
                alert(response.data);
                $ctrl.isModifyUserDisabled = false;

            }
        }, function errorCallback(response) {
            alert("Error : " + response.data);
        });
    };
    usrFac.AddUser = function (newUser, $uibModalInstance, $ctrl) {
        var urlToAdd = '../api/User';
        $http({
            method: 'POST',
            url: urlToAdd,
            data: newUser
        }).then(function successCallback(response) {
            if (response.data == '') {
                alert("User Added Successfully !!!");
                $uibModalInstance.close();
            }
            else {
                alert(response.data);
                $ctrl.isAddUserDisabled = false;

            }
        }, function errorCallback(response) {
            alert("Error : " + response.data);
        });
    };
    usrFac.DeleteProfiles = function (index, $scope) {
        var urlToDelete = '../api/User/' + $scope.users[index].uID;
        $http({
            method: 'DELETE',
            url: urlToDelete
        }).then(function successCallback(response) {
            alert("User Deleted Successfully !!!");
            $scope.users.splice(index, 1);
        }, function errorCallback(response) {
            alert("Error : " + response.data);
        });
    };

    usrFac.GetAllProfilesToAssign = function () {
        var urlToGet = "../api/Profile?pageNumber=-1&profileId=''&getDeleted=false";
        return $http.get(urlToGet);
    };
    usrFac.AssignProfiles = function (uID, selectedProfiles, $uibModalInstance, $ctrl) {
        var urlToUpdate = '../api/User/' + uID;
        $http({
            method: 'PUT',
            url: urlToUpdate,
            data: selectedProfiles
        }).then(function successCallback(response) {
            if (response.data == '') {
                alert("Profiles Assigned Successfully !!!");
                $uibModalInstance.close();
            }
            else {
                alert(response.data);
                $ctrl.isAssignProfilesDisabled = false;
            }
        }, function errorCallback(response) {
            alert("Error : " + response.data);
        });
    };
    return usrFac;
});
