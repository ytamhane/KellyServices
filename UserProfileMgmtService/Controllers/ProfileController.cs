using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UserProfileMgmtBusinessEntities;
using UserProfileMgmtBusinessServices;
using System.Web.Http.Cors;
using System.Diagnostics;
using System.Configuration;

namespace UserProfileMgmtService.Controllers
{
    [EnableCors(origins: "http://localhost:53488 , http://amtrowssoprd04.amer.prd.kellyservices.com", headers: "*", methods: "*")]
    public class ProfileController : ApiController
    {
        private IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }


        // GET: api/Profile
        public HttpResponseMessage Get(int pageNumber, string profileId, bool getDeleted)
        {
            int numberOfRecords = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfRecordsPerPage"].ToString());

            var profiles = _profileService.GetAllProfiles(pageNumber, numberOfRecords, profileId, getDeleted);

            if (profiles != null)
                return Request.CreateResponse(HttpStatusCode.OK, profiles);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No profile found.");
        }

        // GET: api/Profile/5
        public HttpResponseMessage Get(int pId)
        {
            var profiles = _profileService.GetProfileById(pId);

            if (profiles != null)
                return Request.CreateResponse(HttpStatusCode.OK, profiles);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No profile found for this id.");
        }


        // GET: api/Profile/5
        public HttpResponseMessage Get(string profileId)
        {
            var profiles = _profileService.GetProfileById(profileId);

            if (profiles != null)
                return Request.CreateResponse(HttpStatusCode.OK, profiles);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No profile found for this profile id.");
        }

        // POST: api/Profile
        public string Post([FromBody]ProfileEntity _profileEntity)
        {
            _profileEntity.createdOn = DateTime.Today;
            _profileEntity.CreatedBy = User.Identity.Name;
            return _profileService.CreateProfile(_profileEntity);
        }

        // PUT: api/Profile/5
        public HttpResponseMessage Put([FromBody]ProfileEntity _profileEntity)
        {
            _profileEntity.UpdatedOn = DateTime.Today;
            _profileEntity.UpdatedBy = User.Identity.Name;
            var profiles = _profileService.UpdateProfile(_profileEntity);

            if (profiles != null)
                return Request.CreateResponse(HttpStatusCode.OK, profiles);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No profile found.");
        }

        // DELETE: api/Profile/5
        public bool Delete(int id)
        {
            return _profileService.DeleteProfile(id);
        }

        // DELETE: api/Profile/5
        public bool Delete(string profileId)
        {
            return _profileService.DeleteProfile(profileId);
        }

        //[HttpGet]
        //[Route("api/testauthentication")]
        //public IHttpActionResult TestAutentication()
        //{
        //    Debug.Write("AuthenticationType:" + User.Identity.AuthenticationType);
        //    Debug.Write("IsAuthenticated:" + User.Identity.IsAuthenticated);
        //    Debug.Write("Name:" + User.Identity.Name);


        //    if (User.Identity.IsAuthenticated && User.IsInRole("DOMAIN\\UID_UserProfileManage"))
        //    {
        //        return Ok("Authenticated: " + User.Identity.Name);
        //    }
        //    else
        //    {
        //        return BadRequest("Not authenticated");
        //    }
        //}

    }
}
