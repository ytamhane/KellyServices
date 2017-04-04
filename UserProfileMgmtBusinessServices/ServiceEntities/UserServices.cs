using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProfileMgmtBusinessEntities;
using UserProfileMgmtDataModel;
using UserProfileMgmtDataModel.UnitOfWork;
using System.Transactions;

namespace UserProfileMgmtBusinessServices
{
    public class UserServices : IUserServices
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        public UserServices(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string AssignProfiles(int uID, int[] pIDList, string CreatedName, DateTime CreatedDateTime)
        {
            var errorMessage = "";
            if (uID > 0)
            {

                var userCount = _unitOfWork.UserRepository.GetMany(a => a.uID == uID).ToList().Count();
                if (userCount == 1)
                {
                    using (var scope = new TransactionScope())
                    {

                        _unitOfWork.UserProfileRepository.Delete(a => a.uID == uID);
                        for (int i = 0; i < pIDList.Length; i++)
                        {
                            var userProfile = new UserProfile
                            {
                                uID = uID,
                                pID = pIDList[i],
                                CreatedBy = CreatedName,
                                createdOn = CreatedDateTime
                            };
                            _unitOfWork.UserProfileRepository.Insert(userProfile);
                            _unitOfWork.Save();
                        }
                        _unitOfWork.Save();
                        scope.Complete();
                    }
                }
            }
            return errorMessage;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userEntity"></param>
        /// <returns></returns>
        public string CreateUser(UserEntity userEntity)
        {
            var errorMessage = "";
            var userCount = _unitOfWork.UserRepository.GetMany(a => a.UserID.Trim().ToUpper() == userEntity.UserID.Trim().ToUpper()).ToList().Count();
            if (userCount == 0)
            {
                using (var scope = new TransactionScope())
                {
                    var user = new User
                    {
                        Active = true,
                        CreatedBy = userEntity.CreatedBy,
                        createdOn = userEntity.createdOn,
                        UserID = userEntity.UserID,
                        UserName = userEntity.UserName
                    };
                    _unitOfWork.UserRepository.Insert(user);
                    _unitOfWork.Save();

                    //foreach (UserProfileEntity upEntity in userEntity.UserProfileEntities)
                    //{
                    //    var userProfile = new UserProfile
                    //    {
                    //        uID = user.uID,
                    //        pID = upEntity.pID,
                    //        CreatedBy = userEntity.CreatedBy,
                    //        createdOn = userEntity.createdOn
                    //    };
                    //    _unitOfWork.UserProfileRepository.Insert(userProfile);
                    //    _unitOfWork.Save();
                    //}
                    scope.Complete();
                    return errorMessage;
                }
            }
            else
            {
                errorMessage = "Duplicate User Id not allowed.";
                return errorMessage;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool DeleteUser(string userId)
        {
            var success = false;
            var query = _unitOfWork.UserRepository.GetMany(a => a.UserID.Trim().ToUpper() == userId.Trim().ToUpper());
            var userCount = query.Count();

            if (userCount == 1)
            {
                using (var scope = new TransactionScope())
                {
                    User user = query.FirstOrDefault();
                    if (user != null)
                    {
                        user.Active = false;
                        _unitOfWork.UserRepository.Update(user);
                        _unitOfWork.UserProfileRepository.Delete(a => a.uID == user.uID);
                        _unitOfWork.Save();
                        success = true;
                    }
                    scope.Complete();
                }
            }
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public bool DeleteUser(int uId)
        {
            var success = false;
            var query = _unitOfWork.UserRepository.GetMany(a => a.uID == uId);
            var userCount = query.Count();

            if (userCount == 1)
            {
                using (var scope = new TransactionScope())
                {
                    User user = query.FirstOrDefault();
                    if (user != null)
                    {
                        user.Active = false;
                        _unitOfWork.UserRepository.Update(user);
                        //_unitOfWork.UserProfileRepository.Delete(a => a.uID == user.uID);
                        _unitOfWork.Save();

                        success = true;
                    }
                    scope.Complete();
                }
            }
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <param name="numberOfRecords"></param>
        /// <returns></returns>
        public UserEntityWithCount GetAllUsers(int PageNumber, int numberOfRecords, string userId, bool getDeleted)
        {
            int pageNumber = (PageNumber <= 0) ? 1 : PageNumber;
            int NoOfRecords = (numberOfRecords > 50) ? 50 : numberOfRecords;
            int recordsToSkip = NoOfRecords * (PageNumber - 1);

            //string queryCriteria = "";
            var query = _unitOfWork.UserRepository.GetAll();// .GetMany(a => a.UserName.Trim() != "DELETED USER MODEL");

            if ((userId != null) && (userId.Trim() != string.Empty) && (userId.ToUpper() != "UNDEFINED"))
            {
                query = query.Where(a => a.UserID.Trim().ToUpper() == userId.ToUpper());
            }

            if (!getDeleted) //Don't get deleted
            {
                query = query.Where(a => a.Active);
            }

            //var query = _unitOfWork.UserRepository.GetMany(a => a.UserName.Trim() != "DELETED USER MODEL");
            var userCount = query.Count();
            NoOfRecords = (NoOfRecords > userCount) ? userCount : NoOfRecords;
            recordsToSkip = (recordsToSkip > userCount) ? 0 : recordsToSkip;

            var users = query.Skip(recordsToSkip).Take(NoOfRecords).ToList();

            return GetUserEntityFromUserModel(users, userCount, NoOfRecords);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public UserEntityWithCount GetUserById(string userId)
        {
            var users = _unitOfWork.UserRepository.GetMany(a => a.UserID.Trim().ToUpper() == userId.Trim().ToUpper()).ToList();
            var userCount = users.Count();
            return GetUserEntityFromUserModel(users, userCount, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uId"></param>
        /// <returns></returns>
        public UserEntityWithCount GetUserById(int uId)
        {
            var users = _unitOfWork.UserRepository.GetMany(a => a.uID == uId).ToList();
            int userCount = users.Count();
            return GetUserEntityFromUserModel(users.ToList(), userCount, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userEntity"></param>
        /// <returns></returns>
        public string UpdateUser(UserEntity userEntity)
        {
            var errorMessage = "";
            if (userEntity != null)
            {
                //Check if UserID is duplicated
                var duplicateQueryCount = _unitOfWork.UserRepository.GetMany(a => a.UserID.Trim().ToUpper() == userEntity.UserID.Trim().ToUpper()
                                                                        && a.uID != userEntity.uID).ToList().Count();
                if (duplicateQueryCount > 0)
                {
                    errorMessage = "Duplicate User Id not allowed.";
                }
                else
                {
                    var userCount = _unitOfWork.UserRepository.GetMany(a => a.uID == userEntity.uID).ToList().Count();
                    if (userCount == 1)
                    {
                        using (var scope = new TransactionScope())
                        {
                            var user = _unitOfWork.UserRepository.GetByID(userEntity.uID);
                            if (user != null)
                            {
                                user.Active = true;
                                user.UpdatedBy = userEntity.UpdatedBy;
                                user.UpdatedOn = userEntity.UpdatedOn;
                                user.UserID = userEntity.UserID;
                                user.UserName = userEntity.UserName;
                                _unitOfWork.UserRepository.Update(user);

                                //_unitOfWork.UserProfileRepository.Delete(a => a.uID == user.uID);
                                //foreach (UserProfileEntity upEntity in userEntity.UserProfileEntities)
                                //{
                                //    var userProfile = new UserProfile
                                //    {
                                //        uID = user.uID,
                                //        pID = upEntity.pID,
                                //        CreatedBy = userEntity.CreatedBy,
                                //        createdOn = userEntity.createdOn
                                //    };
                                //    _unitOfWork.UserProfileRepository.Insert(userProfile);
                                //    _unitOfWork.Save();
                                //}
                                _unitOfWork.Save();
                                scope.Complete();
                            }
                        }
                    }
                }

            }
            return errorMessage;
        }


        #region Private methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="users"></param>
        /// <param name="userCount"></param>
        /// <returns></returns>
        private UserEntityWithCount GetUserEntityFromUserModel(List<User> users, int userCount, int numberOfRowsPerPage)
        {
            UserEntityWithCount userEntityWithCount = new UserEntityWithCount();
            if (users.Any())
            {
                userEntityWithCount.userEntities = new List<UserEntity>();
                foreach (var user in users)
                {
                    var userEntity = new UserEntity()
                    {
                        uID = user.uID,
                        Active = user.Active,
                        CreatedBy = user.CreatedBy,
                        createdOn = user.createdOn,
                        UpdatedBy = user.UpdatedBy,
                        UpdatedOn = user.UpdatedOn,
                        UserID = user.UserID,
                        UserName = user.UserName
                    };
                    userEntity.UserProfileEntities = new List<UserProfileEntity>();

                    foreach (var userprofile in user.UserProfiles)
                    {
                        var userprofileEntity = new UserProfileEntity()
                        {
                            pID = userprofile.pID,
                            CreatedBy = userprofile.CreatedBy,
                            createdOn = userprofile.createdOn,
                            profileId = userprofile.profile.ProfileId,
                            userId = userprofile.user.UserID
                        };
                        userEntity.UserProfileEntities.Add(userprofileEntity);
                    }

                    userEntityWithCount.userEntities.Add(userEntity);
                }
                userEntityWithCount.userCount = userCount;
                userEntityWithCount.numberOfRowsPerPage = numberOfRowsPerPage;

                return userEntityWithCount;
            }
            return null;


        }

        #endregion



    }
}
