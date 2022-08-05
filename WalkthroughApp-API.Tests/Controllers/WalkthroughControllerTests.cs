using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute.Routing.Handlers;
using NUnit.Framework;
using WalkthroughApp_API.Controllers;
using WalkthroughApp_API.DAL;
using WalkthroughApp_API.DAL.Walkthroughs;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.Tests.Controllers
{
    public class WalkthroughControllerTests
    {
        private DataContext _context;
        private IGet<Walkthrough> _getWalkthroughs;
        private ICreate<Walkthrough, NewWalkthrough> _createWalkthrough;
        private IDelete<Walkthrough> _deleteWalkthrough;
        private IUpdate<Walkthrough> _updateWalkthrough;
        private WalkthroughController _walkthroughController;

        [SetUp]
        public void Setup()
        {
            _context = BuildContext("Walkthrough controller tests");
            _getWalkthroughs = new GetWalkthroughs(_context);
            _createWalkthrough = new CreateWalkthrough(_context);
            _deleteWalkthrough = new DeleteWalkthrough(_context);
            _updateWalkthrough = new UpdateWalkthrough(_context);
            _walkthroughController = new WalkthroughController(_getWalkthroughs, _createWalkthrough, _deleteWalkthrough, _updateWalkthrough);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }
        [Test]
        public async Task Return_all_walkthroughs()
        {
            var sut = await _walkthroughController.GetAll();

            var resultValue = (sut as OkObjectResult).Value as List<Walkthrough>;
            Assert.IsNotNull(resultValue);
            Assert.IsTrue(resultValue[0].Id.Equals(1));
            Assert.IsTrue(resultValue[0].WalkthroughName.Equals("any walkthrough name"));
            Assert.IsTrue(resultValue[0].EmployeeRole.Id.Equals(1));
            Assert.IsTrue(resultValue[0].EmployeeRole.Name.Equals("any job title"));
            Assert.IsTrue(resultValue[1].Id.Equals(2));
            Assert.IsTrue(resultValue[1].WalkthroughName.Equals("any other walkthrough name"));
            Assert.IsTrue(resultValue[1].EmployeeRole.Id.Equals(2));
            Assert.IsTrue(resultValue[1].EmployeeRole.Name.Equals("any other job title"));
        }

        [Test]
        public async Task Return_not_found_message_if_no_walkthroughs_exist()
        {
            var context = BuildEmptyContext("Walkthroughs controller tests empty");
            var getWalkthrough = new GetWalkthroughs(context);
            var walkthroughController = new WalkthroughController(getWalkthrough, _createWalkthrough, _deleteWalkthrough, _updateWalkthrough);

            var sut = await walkthroughController.GetAll();
            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.NotFound, (sut as ObjectResult).StatusCode);
            Assert.AreEqual("No walkthroughs were found", (sut as ObjectResult).Value);
        }

        [TestCase(1, "any walkthrough name")]
        [TestCase(2,  "any other walkthrough name")]
        public void Return_walkthrough_when_walkthrough_id_passed(int walkthroughId, string walkthrough)
        {
            var sut = _walkthroughController.GetById(walkthroughId);

            var resultValue = (sut as OkObjectResult).Value as Walkthrough;
            Assert.IsNotNull(resultValue);
            Assert.IsTrue(resultValue.Id.Equals(walkthroughId));
            Assert.IsTrue(resultValue.WalkthroughName.Equals(walkthrough));
        }

        [Test]
        public async Task Return_not_found_message_if_no_walkthrough_exist_when_walkthrough_id_passed()
        {

            var sut = _walkthroughController.GetById(4);
            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.NotFound, (sut as ObjectResult).StatusCode);
            Assert.AreEqual("No walkthrough was found", (sut as ObjectResult).Value);
        }

        [TestCase("any new walkthrough name", 1)]
        [TestCase("any other new walkthrough name", 2)]
        public async Task Return_created_message_when_new_walkthrough_created(string expectedWalkthroughName, int expectedEmployeeId)
        {
            var newWalkthrough = new NewWalkthrough
            {
                WalkthroughName = expectedWalkthroughName,
                EmployeeRole = expectedEmployeeId

            };

            var sut = await _walkthroughController.AddNewWalkthrough(newWalkthrough);
            var result = sut as CreatedResult;
            var walkthrough = result.Value as Walkthrough;

            var walkthroughs = await _walkthroughController.GetAll();
            var walkthroughCount = (walkthroughs as OkObjectResult).Value as List<Walkthrough>;
            
            Assert.AreEqual(expectedWalkthroughName, walkthrough.WalkthroughName);
            Assert.AreEqual(3, walkthroughCount.Count);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Return_bad_request_when_creating_a_walkthrough_with_no_name(string invalidName)
        {
            var newWalkthrough = new NewWalkthrough
            {
                WalkthroughName = invalidName
            };

            var sut = await _walkthroughController.AddNewWalkthrough(newWalkthrough);

            Assert.IsInstanceOf<BadRequestObjectResult>(sut);
            Assert.AreEqual("Walkthrough name must not be null or empty", (sut as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task Return_created_message_when_creating_walkthrough_with_a_name_that_exists_and_assigned_a_new_employee_role()
        {
            var newWalkthrough = new NewWalkthrough
            {
                WalkthroughName = "any walkthrough name",
                EmployeeRole = 2
            };

            var sut = await _walkthroughController.AddNewWalkthrough(newWalkthrough);
            var result = sut as CreatedResult;
            var walkthrough = result.Value as Walkthrough;

            var walkthroughs = await _walkthroughController.GetAll();
            var walkthroughCount = (walkthroughs as OkObjectResult).Value as List<Walkthrough>;

            Assert.AreEqual("any walkthrough name", walkthrough.WalkthroughName);
            Assert.AreEqual(2, walkthrough.EmployeeRole.Id);
            Assert.AreEqual("any other job title", walkthrough.EmployeeRole.Name);
            Assert.AreEqual(3, walkthroughCount.Count);
        }

        [Test]
        public async Task Return_conflict_message_when_creating_walkthrough_with_a_name_and_employee_role_that_already_exists()
        {
            var newWalkthrough = new NewWalkthrough
            {
                WalkthroughName = "any walkthrough name",
                EmployeeRole = 1
            };

            var sut = await _walkthroughController.AddNewWalkthrough(newWalkthrough);

            Assert.IsNotNull(sut);
            Assert.IsAssignableFrom<ObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.Conflict, (sut as ObjectResult).StatusCode);
        }

        [Test]
        public async Task Return_no_content_when_walkthrough_has_been_deleted()
        {
            var deletedWalkthrough = 1;
            var sut = await _walkthroughController.DeleteWalkthrough(deletedWalkthrough);

            var walkthrough = await _walkthroughController.GetAll();
            var walkthroughCount = (walkthrough as OkObjectResult).Value as List<Walkthrough>;

            Assert.IsInstanceOf<NoContentResult>(sut);
            Assert.AreEqual(1, walkthroughCount.Count);
        }

        [Test]
        public async Task Return_walkthrough_not_found_when_walkthrough_to_be_deleted_does_not_exist()
        {
            var deletedQuestion = 4;
            var sut = await _walkthroughController.DeleteWalkthrough(deletedQuestion);

            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual("Walkthrough does not exist", (sut as NotFoundObjectResult).Value);
        }

        [Test]
        public async Task Return_walkthrough_with_updated_text_after_a_change_has_been_made()
        {
            var updatedWalkthrough = new Walkthrough
            {
                Id = 1,
                WalkthroughName = "updated walkthrough text"
            };
            var sut = await _walkthroughController.UpdateWalkthrough(updatedWalkthrough);

            var walkthrough = _walkthroughController.GetById(1);

            Assert.IsInstanceOf<OkObjectResult>(sut);
            Assert.AreEqual("updated walkthrough text", ((walkthrough as OkObjectResult).Value as Walkthrough).WalkthroughName);
        }

        [Test]
        public async Task Return_question_not_found_when_question_to_be_updated_does_not_exist()
        {
            var updatedWalkthrough = new Walkthrough
            {
                Id = 4,
                WalkthroughName = "updated walkthrough text"
            };
            var sut = await _walkthroughController.UpdateWalkthrough(updatedWalkthrough);

            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual("Walkthrough does not exist", (sut as NotFoundObjectResult).Value);
        }

        private DataContext BuildContext(string key)
        {
            var builder = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(key);
            var context = new DataContext(builder.Options);
            context.Walkthroughs.AddRange(new[]
            {
                new Walkthrough{ Id = 1, WalkthroughName = "any walkthrough name", EmployeeRole = new JobTitle {Id = 1, Name = "any job title"}},
                new Walkthrough{ Id = 2, WalkthroughName = "any other walkthrough name", EmployeeRole = new JobTitle {Id = 2, Name = "any other job title"}}
            });
            context.SaveChanges();

            return context;
        }

        private DataContext BuildEmptyContext(string key)
        {
            var builder = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(key);
            var context = new DataContext(builder.Options);

            return context;
        }
    }
}
