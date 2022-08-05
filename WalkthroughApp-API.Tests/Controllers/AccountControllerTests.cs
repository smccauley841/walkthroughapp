using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NSubstitute.Core;
using WalkthroughApp_API.Controllers;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;
using WalkthroughApp_API.Helpers;
using Shouldly;

namespace WalkthroughApp_API.Tests.Controllers
{
    public class AccountControllerTests
    {
        private AccountController _accountController;
        private IEncodePassword _encodePasswordMock;
        private DataContext _context;
        private byte[] _passwordHash;
        private byte[] _passwordSalt;
        private UserRegistration _newUser;

        [SetUp]
        public void Setup()
        {
            _context = BuildContext("Account Controller Tests");
            _encodePasswordMock = Substitute.For<IEncodePassword>();
            _accountController = new AccountController(_context, _encodePasswordMock);
            _passwordHash = new byte[] { 0x00 };
            _passwordSalt = new byte[] { 0x01 };

            _encodePasswordMock.EncodePassword("any password").Returns(Tuple.Create(_passwordHash, _passwordSalt));

            _newUser = new UserRegistration()
            {
                UserName = "any new username",
                Password = "any password",
                JobTitle = "any job title"
            };
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task Return_user_when_username_and_password_have_been_passed()
        {
            var sut = await _accountController.Register(_newUser);

            Assert.IsInstanceOf<OkObjectResult>(sut);
            var resultValue = (sut as OkObjectResult).Value as User;
            Assert.IsNotNull(resultValue);
            Assert.IsTrue(resultValue.UserName.Equals("any new username"));
            Assert.IsTrue(resultValue.JobTitle.Equals("any job title"));
        }

        [Test]
        public async Task Check_password_has_been_encoded_when_correct_data_passed()
        {
            await _accountController.Register(_newUser);

            _encodePasswordMock.Received().EncodePassword("any password");
        }

        [Test]
        public async Task Return_error_if_username_already_exists()
        {
            var existingUser = new UserRegistration()
            {
                UserName = "any user name 1",
                Password = "any password",
                JobTitle = "any job title"
            }; 
            var sut = await _accountController.Register(existingUser);

            Assert.IsInstanceOf<ObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.Conflict, (sut as ObjectResult).StatusCode);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Return_bad_request_if_username_is_invalid(string username)
        {
            _newUser.UserName = username;

            var sut = await _accountController.Register(_newUser);

            Assert.IsInstanceOf<ObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, (sut as ObjectResult).StatusCode);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Return_bad_request_if_password_is_invalid(string password)
        {
            _newUser.Password = password;

            var sut = await _accountController.Register(_newUser);

            Assert.IsInstanceOf<ObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, (sut as ObjectResult).StatusCode);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Return_bad_request_if_job_title_is_invalid(string jobTitle)
        {
            _newUser.JobTitle = jobTitle;

            var sut = await _accountController.Register(_newUser);

            Assert.IsInstanceOf<ObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, (sut as ObjectResult).StatusCode);
        }

        [Test]
        public async Task Return_user_if_username_and_password_are_correct()
        {
            var userLoginDetails = new LoginDetails()
            {
                Username = "any user name 1",
                Password = "any password"
            };

            _encodePasswordMock
                .EncodePasswordWithKey(Arg.Any<byte[]>(), Arg.Any<string>())
                .Returns(new byte[] { 1 });

            var sut = await _accountController.Login(userLoginDetails);
            var response = (sut as OkObjectResult).Value as User;
            Assert.IsInstanceOf<OkObjectResult>(sut);
            Assert.AreEqual(response.UserName, userLoginDetails.Username);
        }

        [Test]
        public async Task Return_user_not_found_if_username_does_not_exist()
        {
            var userLoginDetails = new LoginDetails()
            {
                Username = "any invalid user",
                Password = "any password"
            };

            var sut = await _accountController.Login(userLoginDetails);

            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.NotFound, (sut as ObjectResult).StatusCode);
        }

        [Test]
        public async Task Return_unauthorised_response_if_password_does_not_match()
        {
            var userLoginDetails = new LoginDetails()
            {
                Username = "any user name 1",
                Password = "any password"
            };

            var wrongPassword = new byte[] { 0, 1, 2, 3 };

            var sut = await _accountController.Login(userLoginDetails);

            _encodePasswordMock
                .EncodePasswordWithKey(Arg.Any<byte[]>(), Arg.Any<string>())
                .Returns(wrongPassword);

            Assert.IsInstanceOf<UnauthorizedObjectResult>(sut);
        }


        private DataContext BuildContext(string key)
        {
            var builder = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(key);
            var context = new DataContext(builder.Options);
            context.Users.AddRange(new[]
            {
                new User(){ Id = 1, UserName = "any user name 1", JobTitle = "any job title", PasswordHash = new byte[]{1}, PasswordSalt = new byte[] { 2 }}
            });
            context.SaveChanges();

            return context;
        }
    }
}
