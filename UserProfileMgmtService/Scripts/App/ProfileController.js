/// <reference path="../Scripts/angular.js" />
/// <reference path="MainAppController.js" />
userProfileModule.controller("profileController", function ($scope, $http, ProfileService, $uibModal) {
    var $ctrl = this;
    $scope.profiles = [];
    $scope.maxSize = 10;
    $scope.currentPage = 1;
    $scope.profileId = '';
    $scope.getDeleted = false;
    $scope.selectedProfile = [];


    //Get data for page when the page shows up for the first time
    GetDataForPage($scope, $http, ProfileService, $scope.currentPage, $scope.profileId, $scope.getDeleted);

    //Page change event handled
    $scope.pageChanged = function () {
        GetDataForPage($scope, $http, ProfileService, $scope.currentPage, $scope.profileId, $scope.getDeleted);
    };

    //When user searches for a profile id
    $scope.SearchProfileId = function (profileid) {
        $scope.profileId = profileid;
        GetDataForPage($scope, $http, ProfileService, $scope.currentPage, $scope.profileId, $scope.getDeleted);

    };

    $scope.DeleteProfiles = function (index) {
        ProfileService.DeleteProfiles(index, $scope);
    };


    //On double clicking of a row, the profile is shown in this dialog box
    $ctrl.modify = function (size, parentSelector, profile) {
        $ctrl.selectedProfile = profile;
        var parentElem = undefined; 
        var modalInstance = $uibModal.open({
            animation: $ctrl.animationsEnabled,
            ariaLabelledBy: 'modify-profile-modal-title',
            ariaDescribedBy: 'modify-profile-modal-body',
            templateUrl: 'ModifyProfileModal.html',
            controller: 'ModifyProfileInstanceCtrl',
            controllerAs: '$ctrl',
            size: size,
            appendTo: parentElem,
            resolve: {
                selectedProfileToModify: function () {
                    return $ctrl.selectedProfile;
                }
            }
        });

        modalInstance.result.then(function (response) {
            location.reload();
        }, function () {
        });
    };

    $ctrl.addNew = function (size) {
        var parentElem = undefined;
        var modalAddInstance = $uibModal.open({
            animation: $ctrl.animationsEnabled,
            ariaLabelledBy: 'add-profile-modal-title',
            ariaDescribedBy: 'add-profile-modal-body',
            templateUrl: 'AddProfileModal.html',
            controller: 'AddProfileInstanceCtrl',
            controllerAs: '$ctrl',
            size: size,
            appendTo: parentElem
        });

        modalAddInstance.result.then(function () {
            location.reload();
        }, function () {
        });
    };

});

//controller for modify profile 
angular.module('userProfileModule').controller('ModifyProfileInstanceCtrl', function ($uibModalInstance, selectedProfileToModify, ProfileService, $scope) {
    var $ctrl = this;
    $ctrl.selectedProfileToModify = {
        pID: selectedProfileToModify.pID,
        ProfileId: selectedProfileToModify.ProfileId,
        Description: selectedProfileToModify.Description,
        UpdatedBy: selectedProfileToModify.UpdatedBy,
        UpdatedOn: selectedProfileToModify.UpdatedOn
    };

    $ctrl.validateMessage = '';
    $ctrl.retProfile = '';
    $ctrl.isModifyProfileDisabled = false;

    $ctrl.ok = function () {
        validateMessage = Validate($ctrl.selectedProfileToModify);
        if (validateMessage == '') {
            ProfileService.UpdateProfile($ctrl.selectedProfileToModify, $uibModalInstance, $ctrl);
        }
        else {
            $ctrl.isModifyProfileDisabled = false;
            alert(validateMessage);
        }
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };


});

//controller for add profile 
angular.module('userProfileModule').controller('AddProfileInstanceCtrl', function ($uibModalInstance, ProfileService) {
    var $ctrl = this;
    $ctrl.addNewProfileData = {
        pID: 0,
        ProfileId: '',
        Description: ''
    };

    $ctrl.validateMessage = '';
    $ctrl.isAddProfileDisabled = false;

    $ctrl.AddNewProfile = function () {
        validateMessage = Validate($ctrl.addNewProfileData);
        if (validateMessage == '') {
            ProfileService.AddProfile($ctrl.addNewProfileData, $uibModalInstance, $ctrl);
        }
        else {
            $ctrl.isAddProfileDisabled = false;
            alert(validateMessage);
        }
    };

    $ctrl.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});

//Validate profile id and description on Modify or Add New 
function Validate(selectedProfile) {
    var validateMessage = '';
    if (selectedProfile.ProfileId.trim() === '') {
        validateMessage += 'Profile ID cannot be blank. \n';
    }

    var splChars = "*|,\":<>[]{}`\';()@&$#%";
    for (var i = 0; i < selectedProfile.ProfileId.length; i++) {
        if (splChars.indexOf(selectedProfile.ProfileId.charAt(i)) != -1) {
            validateMessage += 'Profile Id cannot have special characters. \n';
            break;
        }
    }

    if (selectedProfile.Description.trim() === '') {
        validateMessage += 'Profile Description cannot be blank. \n';
    }

    var splChars = "*|,\":<>[]{}`\';()@&$#%";
    for (var i = 0; i < selectedProfile.Description.length; i++) {
        if (splChars.indexOf(selectedProfile.Description.charAt(i)) != -1) {
            validateMessage += 'Profile Description cannot have special characters. \n';
            break;
        }
    }
    return validateMessage;
}

function GetDataForPage($scope, $http, ProfileService, pageNumber, profileId, getDeleted) {
    ProfileService.GetAllProfiles(pageNumber, profileId, getDeleted)
			.then(
				function (response) {
				    $scope.profiles = response.data.profileEntities;
				    $scope.totalItems = response.data.profileCount;
				    $scope.currentPage = pageNumber;
				    $scope.errorMessage = '';
				    $scope.numberOfRowsPerPage = response.data.numberOfRowsPerPage;
				},
                function (error) {
                    $scope.errorMessage = "No Data Found";
                    $scope.profiles = [];
                    $scope.totalItems = 0;
                    $scope.currentPage = 1;
                    $scope.numberOfRowsPerPage = 0;
                });

}


userProfileModule.factory('ProfileService', function ($http) {
    var prfFac = {};
    prfFac.GetAllProfiles = function (pageNumber, profileId, getDeleted) {
        var urlToGet = '../api/Profile?pageNumber=' + pageNumber + "&profileId=" + profileId + "&getDeleted=" + getDeleted;
        return $http.get(urlToGet);
    };

    prfFac.UpdateProfile = function (updatedProfile, $uibModalInstance, $ctrl) {
        var urlToUpdate = '../api/Profile';
        $http({
            method: 'PUT',
            url: urlToUpdate,
            data: updatedProfile
        }).then(function successCallback(response) {
            if (response.data == '') {
                alert("Profile Updated Successfully !!!");
                $uibModalInstance.close();
            }
            else {
                alert(response.data);
                $ctrl.isModifyProfileDisabled = false;
            }
        }, function errorCallback(response) {
            alert("Error : " + response.data);
        });
    };
    prfFac.AddProfile = function (newProfile, $uibModalInstance, $ctrl) {
        var urlToAdd = '../api/Profile';
        $http({
            method: 'POST',
            url: urlToAdd,
            data: newProfile
        }).then(function successCallback(response) {
            if (response.data == '') {
                alert("Profile Added Successfully !!!");
                $uibModalInstance.close();
            }
            else {
                alert(response.data);
                $ctrl.isAddProfileDisabled = false;
            }
        }, function errorCallback(response) {
            alert("Error : " + response.data);
        });
    };
    prfFac.DeleteProfiles = function (index, $scope) {
        var urlToDelete = '../api/Profile/' + $scope.profiles[index].pID;
        $http({
            method: 'DELETE',
            url: urlToDelete
        }).then(function successCallback(response) {
            alert("Profile Deleted Successfully !!!");
            $scope.profiles.splice(index, 1);
        }, function errorCallback(response) {
            alert("Error : " + response.data);
        });
    };
    return prfFac;
});