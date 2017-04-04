using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UserProfileMgmtBusinessServices;
using UserProfileMgmtBusinessEntities;
using System.Web.Http.Cors;
using System.Configuration;

namespace UserProfileMgmtService.Controllers
{
    [EnableCors(origins: "http://localhost:53488 , http://amtrowssoprd04.amer.prd.kellyservices.com", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        // GET: api/User

        public HttpResponseMessage Get(int pageNumber, string userId, bool getDeleted)
        {
            int numberOfRecords = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfRecordsPerPage"].ToString());

           var users = _userServices.GetAllUsers(pageNumber, numberOfRecords, userId, getDeleted);

            if (users != null)
                return Request.CreateResponse(HttpStatusCode.OK, users);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No User found for this id");
        }

        // GET: api/User/5
        public HttpResponseMessage Get(int id)
        {
            var user = _userServices.GetUserById(id);
            if (user != null)
                return Request.CreateResponse(HttpStatusCode.OK, user);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No User found for this id");
        }

        // GET: api/User/5
        public HttpResponseMessage Get(string userId)
        {
            var user = _userServices.GetUserById(userId);
            if (user != null)
                return Request.CreateResponse(HttpStatusCode.OK, user);
            return Request.CreateErrorResponse(HttpStatusCode.NotFound, "No User found for this id");
        }

        // POST: api/User
        public string Post([FromBody]UserEntity userEntity)
        {
            userEntity.createdOn = DateTime.Today;
            userEntity.CreatedBy = User.Identity.Name;
            return _userServices.CreateUser(userEntity);
        }

        // PUT: api/User
        public string Put([FromBody]UserEntity userEntity)
        {
            userEntity.UpdatedOn = DateTime.Today;
            userEntity.UpdatedBy = User.Identity.Name;

            return _userServices.UpdateUser(userEntity);
        }
        public string Put(int id, [FromBody]int[] selectedProfiles)
        {
            return _userServices.AssignProfiles(id, selectedProfiles, User.Identity.Name, DateTime.Today);
            //userEntity.UpdatedOn = DateTime.Today;
            //userEntity.UpdatedBy = User.Identity.Name;

            //return _userServices.UpdateUser(userEntity);
        }
        // DELETE: api/User/5
        public bool Delete(int id)
        {
            return _userServices.DeleteUser(id);
        }

        public bool Delete(string userId)
        {
            return _userServices.DeleteUser(userId);
        }

    }
}
