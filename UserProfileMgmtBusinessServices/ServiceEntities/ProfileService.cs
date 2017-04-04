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
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProfileService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public string CreateProfile(ProfileEntity profileEntity)
        {
            var errorMessage = "";
            var profileCount = _unitOfWork.ProfileRepository.GetMany(a => a.ProfileId.Trim().ToUpper() == profileEntity.ProfileId.Trim().ToUpper()).ToList().Count();
            if (profileCount == 0)
            {
                using (var scope = new TransactionScope())
                {
                    var profile = new Profile
                    {
                        CreatedBy = profileEntity.CreatedBy,
                        createdOn = profileEntity.createdOn,
                        ProfileId = profileEntity.ProfileId,
                        Description = profileEntity.Description,
                        Enabled = true
                    };
                    _unitOfWork.ProfileRepository.Insert(profile);
                    _unitOfWork.Save();
                    scope.Complete();
                    return errorMessage;
                }
            }
            else
            {
                errorMessage = "Duplicate profile id not allowed.";
                return errorMessage;
            }
        }

        public bool DeleteProfile(string profileId)
        {
            var query = _unitOfWork.ProfileRepository.GetMany(a => a.ProfileId.Trim().ToUpper() == profileId.Trim().ToUpper());

            return DisableProfile(query);
        }

        public bool DeleteProfile(int pId)
        {
            var query = _unitOfWork.ProfileRepository.GetMany(a => a.pID == pId);

            return DisableProfile(query);
        }



        public ProfileEntityWithCount GetAllProfiles(int PageNumber, int numberOfRecords, string profileId, bool getDeleted)
        {
            if (PageNumber == -1)
            {
                var queryAll = _unitOfWork.ProfileRepository.GetAll().Where(a => a.Enabled);
                var profilesAll = queryAll.ToList();
                return GetProfileEntityFromUserModel(profilesAll, queryAll.Count(), queryAll.Count());

            }
            else
            {
                int pageNumber = (PageNumber <= 0) ? 1 : PageNumber;
                int NoOfRecords = (numberOfRecords > 50) ? 50 : numberOfRecords;
                int recordsToSkip = NoOfRecords * (PageNumber - 1);

                //string queryCriteria = "";
                var query = _unitOfWork.ProfileRepository.GetAll();// .GetMany(a => a.UserName.Trim() != "DELETED USER MODEL");

                if ((profileId != null) && (profileId.Trim() != string.Empty) && (profileId.ToUpper() != "UNDEFINED"))
                {
                    query = query.Where(a => a.ProfileId.Trim().ToUpper() == profileId.ToUpper());
                }

                if (!getDeleted) //Don't get deleted
                {
                    query = query.Where(a => a.Enabled);
                }

                //var query = _unitOfWork.UserRepository.GetMany(a => a.UserName.Trim() != "DELETED USER MODEL");
                var profileCount = query.Count();
                NoOfRecords = (NoOfRecords > profileCount) ? profileCount : NoOfRecords;
                recordsToSkip = (recordsToSkip > profileCount) ? 0 : recordsToSkip;

                var profiles = query.Skip(recordsToSkip).Take(NoOfRecords).ToList();

                return GetProfileEntityFromUserModel(profiles, profileCount, NoOfRecords);
            }
        }



        public ProfileEntityWithCount GetProfileById(string profileId)
        {
            var profiles = _unitOfWork.ProfileRepository.GetMany(a => a.ProfileId.Trim().ToUpper() == profileId.Trim().ToUpper()).ToList();
            var profileCount = profiles.Count();
            return GetProfileEntityFromUserModel(profiles, profileCount, 1);
        }

        public ProfileEntityWithCount GetProfileById(int pId)
        {
            var profiles = _unitOfWork.ProfileRepository.GetMany(a => a.pID == pId).ToList();
            var profileCount = profiles.Count();
            return GetProfileEntityFromUserModel(profiles, profileCount, 1);
        }

        public string UpdateProfile(ProfileEntity profileEntity)
        {
            var errorMessage = "";
            if (profileEntity != null)
            {
                //Check if the profile id is duplicated
                var duplicateQuery = _unitOfWork.ProfileRepository.GetMany(a => a.ProfileId.Trim().ToUpper() == profileEntity.ProfileId.Trim().ToUpper()
                                                                            && a.pID != profileEntity.pID);
                if (duplicateQuery.ToList().Count > 0)
                {
                    errorMessage = "Duplicate profile id not allowed.";
                }
                else
                {
                    var query = _unitOfWork.ProfileRepository.GetMany(a => a.pID == profileEntity.pID);
                    var profileCount = query.ToList().Count();
                    if (profileCount == 1)
                    {
                        using (var scope = new TransactionScope())
                        {
                            var profile = query.FirstOrDefault();
                            if (profile != null)
                            {
                                profile.UpdatedBy = profileEntity.UpdatedBy;
                                profile.UpdatedOn = profileEntity.UpdatedOn;
                                profile.ProfileId = profileEntity.ProfileId;
                                profile.Description = profileEntity.Description;
                                _unitOfWork.ProfileRepository.Update(profile);
                                _unitOfWork.Save();
                                scope.Complete();
                            }
                        }
                      }
                }
            }



            return errorMessage;
        }

        #region Private Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="profiles"></param>
        /// <param name="profileCount"></param>
        /// <returns></returns>
        private ProfileEntityWithCount GetProfileEntityFromUserModel(List<Profile> profiles, int profileCount, int numberOfRowsPerPage)
        {
            ProfileEntityWithCount profileEntityWithCount = new ProfileEntityWithCount();
            if (profiles.Any())
            {
                profileEntityWithCount.profileEntities = new List<ProfileEntity>();
                foreach (var profile in profiles)
                {
                    var profileEntity = new ProfileEntity()
                    {
                        pID = profile.pID,
                        CreatedBy = profile.CreatedBy,
                        createdOn = profile.createdOn,
                        UpdatedBy = profile.UpdatedBy,
                        UpdatedOn = profile.UpdatedOn,
                        ProfileId = profile.ProfileId,
                        Description = profile.Description,
                        Enabled = profile.Enabled
                    };

                    profileEntityWithCount.profileEntities.Add(profileEntity);
                }
                profileEntityWithCount.profileCount = profileCount;
                profileEntityWithCount.numberOfRowsPerPage = numberOfRowsPerPage;

                return profileEntityWithCount;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        private bool DisableProfile(IEnumerable<Profile> query)
        {
            var success = false;
            var profileCount = query.ToList().Count();
            if (profileCount == 1)
            {
                using (var scope = new TransactionScope())
                {
                    var profile = query.FirstOrDefault();
                    if (profile != null)
                    {
                        profile.Enabled = false;
                        _unitOfWork.ProfileRepository.Update(profile);
                        _unitOfWork.Save();

                        _unitOfWork.UserProfileRepository.Delete(a => a.pID == profile.pID);

                        _unitOfWork.Save();
                        scope.Complete();
                        success = true;
                    }
                }
            }
            return success;
        }
        #endregion
    }
}
