using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserProfileMgmtDataModel;
using UserProfileMgmtDataModel.UnitOfWork;
using UserProfileMgmtDataModel.GenericRepository;
using NUnit.Framework;
using TestsHelper;
using Moq;

namespace UserProfileMgmtBusinessServices.Tests
{
    public class UserServicesTests
    {
        #region Variables
        private IUserServices _userService;
        private IUnitOfWork _unitOfWork;
        private List<User> _users;
        private GenericRepository<User> _userRepository;
        private UserProfileMgmtEntities _dbEntities;
        #endregion

        #region Test Fixture Setup

        /// <summary>
        /// Initial setup for tests
        /// </summary>
        [OneTimeSetUp]
        public void Setup()
        {
            _users = SetUpUsers();
        }


        #endregion


        private List<User> SetUpUsers()
        {
            var uId = new int();
            var users = DataInitializer.GetAllUsers();
            foreach (User user in users)
            {
                user.uID = ++uId;
            }
            return users;
        }

        #region TestFixture TearDown.
        /// <summary>
        /// TestFixture teardown
        /// </summary>
        [OneTimeTearDown]
        public void DisposeAllObjects()
        {
            _users = null;
        }
        #endregion

        #region Setup
        /// <summary>
        /// Re-initializes test.
        /// </summary>
        [SetUp]
        public void ReInitializeTest()
        {
            _dbEntities = new Mock<UserProfileMgmtEntities>().Object;
            _userRepository = SetUpUserRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.UserRepository).Returns(_userRepository);
            _unitOfWork = unitOfWork.Object;
            _userService = new UserServices(_unitOfWork);
        }
        #endregion

        /// <summary>
        /// Tears down each test data
        /// </summary>
        [TearDown]
        public void DisposeTest()
        {
            _userService = null;
            _unitOfWork = null;
            _userRepository = null;
            if (_dbEntities != null)
                _dbEntities.Dispose();
        }

        private GenericRepository<User> SetUpUserRepository()
        {
            // Initialise repository
            var mockRepo = new Mock<GenericRepository<User>>(MockBehavior.Default, _dbEntities);
            // Setup mocking behavior
            mockRepo.Setup(p => p.GetAll()).Returns(_users);
            mockRepo.Setup(p => p.GetByID(It.IsAny<int>())).Returns(new Func<int, User>(id => _users.Find(p => p.uID.Equals(id))));
            mockRepo.Setup(p => p.Insert((It.IsAny<User>())))
            .Callback(new Action<User>(newUser =>
            {
                dynamic maxUserID = _users.Last().uID;
                dynamic nextUserID = maxUserID + 1;
                newUser.uID = nextUserID;
                _users.Add(newUser);
            }));
            mockRepo.Setup(p => p.Update(It.IsAny<User>()))
            .Callback(new Action<User>(usr =>
            {
                var oldUser = _users.Find(a => a.uID == usr.uID);
                oldUser = usr;
            }));
            mockRepo.Setup(p => p.Delete(It.IsAny<User>()))
            .Callback(new Action<User>(usr =>
            {
                var userToRemove =
    _users.Find(a => a.uID == usr.uID);
                if (userToRemove != null)
                    _users.Remove(userToRemove);
            }));
            // Return mock implementation object
            return mockRepo.Object;
        }
    }
}
