using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WalkthroughApp_API.Controllers;
using WalkthroughApp_API.DAL;
using WalkthroughApp_API.DAL.Choices;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;
using WalkthroughApp_API.Tests.BuildTestDatabase;

namespace WalkthroughApp_API.Tests.Controllers
{
    public class ChoicesControllerTests
    {
        private CreateTestDatabase _createTestDatabase;
        private DataContext _context;
        private IGetChoices _getChoices;
        private ICreateChoices _createChoices;
        private IDelete<Choice> _deleteChoice;
        private IUpdate<Choice> _updateChoice;
        private ChoicesController _choicesController;

        [SetUp]
        public void Setup()
        {
            _createTestDatabase = new CreateTestDatabase();
            _context = _createTestDatabase.BuildContext("Choice controller tests");
            _getChoices = new GetChoices(_context);
            _createChoices = new CreateChoices(_context);
            _deleteChoice = new DeleteChoices(_context);
            _updateChoice = new UpdateChoice(_context);
            _choicesController = new ChoicesController(_getChoices, _createChoices, _deleteChoice, _updateChoice);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task Return_all_job_titles()
        {
            var sut = await _choicesController.GetAll();

            var resultValue = (sut as OkObjectResult).Value as List<Choice>;
            Assert.IsNotNull(resultValue);
            Assert.AreEqual(8, resultValue.Count);
        }

        [Test]
        public async Task Return_not_found_message_if_no_choices_exist()
        {
            var context = BuildEmptyContext("Choice controller tests empty");
            var getChoices = new GetChoices(context);
            _choicesController = new ChoicesController(_getChoices, _createChoices, _deleteChoice, _updateChoice);
            var choicesController = new ChoicesController(getChoices, _createChoices, _deleteChoice, _updateChoice);

            var sut = await choicesController.GetAll();
            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.NotFound, (sut as ObjectResult).StatusCode);
            Assert.AreEqual("No choices were found", (sut as ObjectResult).Value);
        }

        [TestCase(1, "choice 1", true)]
        [TestCase(2, "choice 2", false)]
        [TestCase(5, "choice 1", false)]
        [TestCase(6, "choice 2", false)]
        public async Task Return_choice_when_choice_id_passed(int jobTitleId, string jobTitle, bool isAnswer)
        {
            var sut = await _choicesController.GetById(jobTitleId);

            var resultValue = (sut as OkObjectResult).Value as Choice;
            Assert.IsNotNull(resultValue);
            Assert.IsTrue(resultValue.Id.Equals(jobTitleId));
            Assert.IsTrue(resultValue.ChoiceText.Equals(jobTitle));
            Assert.IsTrue(resultValue.IsAnswer.Equals(isAnswer));
        }

        [Test]
        public async Task Return_not_found_message_if_no_job_exists_when_job_id_passed()
        {

            var sut = await _choicesController.GetById(15);
            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.NotFound, (sut as ObjectResult).StatusCode);
            Assert.AreEqual("No job title was found", (sut as ObjectResult).Value);
        }

        [Test]
        public async Task Return_created_message_when_new_choices_created()
        {
            NewChoice[] newChoices = { new NewChoice{ChoiceText = "choice 5", Question = 2, IsAnswer = false}, new NewChoice { ChoiceText = "choice 6", Question = 2, IsAnswer = false }, new NewChoice { ChoiceText = "choice 7", Question = 2, IsAnswer = false } };

            await _choicesController.AddQuestionChoices(newChoices);
            

            var jobTitles = await _choicesController.GetByQuestionId(2);
            var jobTitlesCount = (jobTitles as OkObjectResult).Value as List<Choice>;

            Assert.That(jobTitlesCount.Any(x => x.ChoiceText == "choice 5"));
            Assert.That(jobTitlesCount.Any(x => x.ChoiceText == "choice 6"));
            Assert.That(jobTitlesCount.Any(x => x.ChoiceText == "choice 7"));
            Assert.AreEqual(7, jobTitlesCount.Count);
        }

        [TestCase("")]
        [TestCase(null)]
        public async Task Return_bad_request_when_creating_a_choice_with_no_text(string invalidName)
        {
            NewChoice[] newChoices = { new NewChoice { ChoiceText = invalidName, Question = 2, IsAnswer = false }, new NewChoice { ChoiceText = "choice 6", Question = 2, IsAnswer = false }, new NewChoice { ChoiceText = "choice 7", Question = 2, IsAnswer = false } };

            var sut = await _choicesController.AddQuestionChoices(newChoices);

           Assert.IsInstanceOf<BadRequestObjectResult>(sut);
           Assert.AreEqual("Choice text must not be null or empty on any choice", (sut as BadRequestObjectResult).Value);
        }

        [Test]
        public async Task Return_conflict_message_when_creating_question_choice_that_already_exists()
        {
            NewChoice[] newChoices = { new NewChoice { ChoiceText = "choice 1", Question = 2, IsAnswer = true }};

            var sut = await _choicesController.AddQuestionChoices(newChoices);

            Assert.IsNotNull(sut);
            Assert.IsAssignableFrom<ObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.Conflict, (sut as ObjectResult).StatusCode);
        }
        [Test]
        public async Task Return_conflict_message_when_creating_question_choice_as_correct_answer_when_the_correct_answer_already_exists()
        {
            NewChoice[] newChoices = { new NewChoice { ChoiceText = "choice 6", Question = 2, IsAnswer = true } };

            var sut = await _choicesController.AddQuestionChoices(newChoices);

            Assert.IsNotNull(sut);
            Assert.IsAssignableFrom<ObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.Conflict, (sut as ObjectResult).StatusCode);
            Assert.AreEqual("A correct answer already exists for this question", (sut as ObjectResult).Value);
        }

        [Test]
        public async Task Return_no_content_when_choice_has_been_deleted()
        {
            var deletedChoice = 1;
            var sut = await _choicesController.DeleteChoice(deletedChoice);

            var choices = await _choicesController.GetAll();
            var choicesCount = (choices as OkObjectResult).Value as List<Choice>;

            Assert.IsInstanceOf<NoContentResult>(sut);
            Assert.AreEqual(7, choicesCount.Count);
            Assert.IsFalse(choicesCount.Any(x => x.Id == 1));
        }

        [Test]
        public async Task Return_choice_not_found_when_choice_to_be_deleted_does_not_exist()
        {
            var deletedChoice = 14;
            var sut = await _choicesController.DeleteChoice(deletedChoice);

            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual("Choice does not exist", (sut as NotFoundObjectResult).Value);
        }

        [Test]
        public async Task Return_walkthrough_with_updated_text_after_a_change_has_been_made()
        {
            var updatedChoice = new Choice
            {
                Id = 1,
                ChoiceText = "updated choice text"
            };

            var sut = await _choicesController.UpdateChoice(updatedChoice);
            
            var choice = await _choicesController.GetById(1);
            
            Assert.IsInstanceOf<OkObjectResult>(sut);
            Assert.AreEqual("updated choice text", ((choice as OkObjectResult).Value as Choice).ChoiceText);
        }

        [Test]
        public async Task Return_question_not_found_when_question_to_be_updated_does_not_exist()
        {
            var updatedChoice = new Choice
            {
                Id = 14,
                ChoiceText = "updated choice text"
            };
            var sut = await _choicesController.UpdateChoice(updatedChoice);
            
            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual("Choice does not exist", (sut as NotFoundObjectResult).Value);
        }

        private DataContext BuildEmptyContext(string key)
        {
            var builder = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(key);
            var context = new DataContext(builder.Options);

            return context;
        }
    }
}
