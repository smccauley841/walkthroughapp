using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using WalkthroughApp_API.Controllers;
using WalkthroughApp_API.DAL;
using WalkthroughApp_API.DAL.JobTitles;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.Tests.Controllers
{
    public class JobTitleControllerTests
    {
        private DataContext _context;
        private IGet<JobTitle> _getJobTitles;
        private ICreate<JobTitle, NewJobTitle> _createJobTitle;
        private IDelete<JobTitle> _deleteJobTitle;
        private IUpdate<JobTitle> _updateJobTitle;
        private JobTitleController _jobTitleController;

        [SetUp]
        public void Setup()
        {
            _context = BuildContext("Job title controller tests");
            _getJobTitles = new GetJobs(_context);
            _createJobTitle = new CreateJob(_context);
            _deleteJobTitle = new DeleteJob(_context);
            _updateJobTitle = new UpdateJob(_context);
            _jobTitleController = new JobTitleController(_getJobTitles,_createJobTitle, _deleteJobTitle, _updateJobTitle);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task Return_all_job_titles()
        {
            var sut = await _jobTitleController.GetAll();

            var resultValue = (sut as OkObjectResult).Value as List<JobTitle>;
            Assert.IsNotNull(resultValue);
            Assert.IsTrue(resultValue[0].Id.Equals(1));
            Assert.IsTrue(resultValue[0].Name.Equals("any job title"));
            Assert.IsTrue(resultValue[1].Id.Equals(2));
            Assert.IsTrue(resultValue[1].Name.Equals("any other job title"));
        }

        [Test]
        public async Task Return_not_found_message_if_no_job_titles_exist()
        {
            var context = BuildEmptyContext("Job title controller tests empty");
            var getJobs = new GetJobs(context);
            var jobTitleController = new JobTitleController(getJobs, _createJobTitle, _deleteJobTitle, _updateJobTitle);

            var sut = await jobTitleController.GetAll();
            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.NotFound, (sut as ObjectResult).StatusCode);
            Assert.AreEqual("No job titles were found", (sut as ObjectResult).Value);
        }

        [TestCase(1, "any job title")]
        [TestCase(2, "any other job title")]
        public async Task Return_walkthrough_when_walkthrough_id_passed(int jobTitleId, string jobTitle)
        {
            var sut = await _jobTitleController.GetById(jobTitleId);

            var resultValue = (sut as OkObjectResult).Value as JobTitle;
            Assert.IsNotNull(resultValue);
            Assert.IsTrue(resultValue.Id.Equals(jobTitleId));
            Assert.IsTrue(resultValue.Name.Equals(jobTitle));
        }

        [Test]
        public async Task Return_not_found_message_if_no_job_exists_when_job_id_passed()
        {

            var sut = await _jobTitleController.GetById(4);
            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.NotFound, (sut as ObjectResult).StatusCode);
            Assert.AreEqual("No job title was found", (sut as ObjectResult).Value);
        }

        [TestCase("any new job name")]
        [TestCase("any other new job name")]
        public async Task Return_created_message_when_new_job_title_created(string expectedJobTitle)
        {
            var newJobTitle = new NewJobTitle
            {
                Name = expectedJobTitle

            };

            var sut = await _jobTitleController.AddNewJobTitle(newJobTitle);
            var result = sut as CreatedResult;
            var jobTitle = result.Value as JobTitle;

            var jobTitles = await _jobTitleController.GetAll();
            var jobTitlesCount = (jobTitles as OkObjectResult).Value as List<JobTitle>;

            Assert.AreEqual(expectedJobTitle, jobTitle.Name);
            Assert.AreEqual(3, jobTitlesCount.Count);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Return_bad_request_when_creating_a_walkthrough_with_no_name(string invalidName)
        {
            var newWalkthrough = new NewJobTitle
            {
                Name = invalidName
            };

            var sut = await _jobTitleController.AddNewJobTitle(newWalkthrough);

            Assert.IsInstanceOf<BadRequestObjectResult>(sut);
            Assert.AreEqual("Job title name must not be null or empty", (sut as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task Return_conflict_message_when_creating_job_title_with_a_name_that_already_exists()
        {
            var newJobTitle = new NewJobTitle
            {
                Name = "any job title"
            };

            var sut = await _jobTitleController.AddNewJobTitle(newJobTitle);

            Assert.IsNotNull(sut);
            Assert.IsAssignableFrom<ObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.Conflict, (sut as ObjectResult).StatusCode);
        }

        [Test]
        public async Task Return_no_content_when_job_has_been_deleted()
        {
            var deletedJob = 1;
            var sut = await _jobTitleController.DeleteJob(deletedJob);

            var jobTitles = await _jobTitleController.GetAll();
            var jobTitlesCount = (jobTitles as OkObjectResult).Value as List<JobTitle>;

            Assert.IsInstanceOf<NoContentResult>(sut);
            Assert.AreEqual(1, jobTitlesCount.Count);
        }

        [Test]
        public async Task Return_not_found_message_when_job_to_be_deleted_does_not_exist()
        {
            var deletedJob = 4;
            var sut = await _jobTitleController.DeleteJob(deletedJob);

            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual("Job does not exist", (sut as NotFoundObjectResult).Value);
        }

        [Test]
        public async Task Return_job_with_updated_name_after_a_change_has_been_made()
        {
            var updateJobTitle = new JobTitle
            {
                Id = 1,
                Name = "updated job name"
            };
            var sut = await _jobTitleController.UpdateJobTitle(updateJobTitle);

            var jobTitle = await _jobTitleController.GetById(1);

            Assert.IsInstanceOf<OkObjectResult>(sut);
            Assert.AreEqual("updated job name", ((jobTitle as OkObjectResult).Value as JobTitle).Name);
        }

        [Test]
        public async Task Return_question_not_found_when_question_to_be_updated_does_not_exist()
        {
            var updateJobTitle = new JobTitle
            {
                Id = 4,
                Name = "updated job name"
            };
            var sut = await _jobTitleController.UpdateJobTitle(updateJobTitle);

            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual("Job does not exist", (sut as NotFoundObjectResult).Value);
        }

        private DataContext BuildContext(string key)
        {
            var builder = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(key);
            var context = new DataContext(builder.Options);
            context.JobTitles.AddRange(new[]
            {
                new JobTitle{ Id = 1, Name = "any job title"},
                new JobTitle {Id = 2, Name = "any other job title"}
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
