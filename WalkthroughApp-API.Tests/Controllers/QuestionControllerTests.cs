using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NSubstitute.Core.Arguments;
using WalkthroughApp_API.Controllers;
using WalkthroughApp_API.DAL;
using WalkthroughApp_API.DAL.Questions;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.Tests.Controllers
{
    public class QuestionControllerTests
    {
        private QuestionsController _questionsController;
        private IGetQuestions _getQuestion;
        private ICreate<Question, NewQuestion> _createQuestion;
        private IDelete<Question> _deleteQuestion;
        private IUpdate<Question> _updateQuestion;
        private DataContext _context;

        [SetUp]
        public void Setup()
        {
            _context = BuildContext("Questions controller tests");
            _getQuestion = new GetQuestions(_context);
            _createQuestion = new CreateQuestion(_context);
            _deleteQuestion = new DeleteQuestion(_context, _getQuestion);
            _updateQuestion = new UpdateQuestion(_context);
            _questionsController = new QuestionsController(_getQuestion, _createQuestion, _deleteQuestion, _updateQuestion);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task Return_all_questions()
        {
            var sut = await _questionsController.GetAll();

            var resultValue = (sut as OkObjectResult).Value as List<Question>;
            Assert.IsNotNull(resultValue);
            Assert.IsTrue(resultValue[0].Id.Equals(1));
            Assert.IsTrue(resultValue[0].QuestionText.Equals("any question"));
            Assert.IsTrue(resultValue[0].Walkthrough.Id.Equals(1));
            Assert.IsTrue(resultValue[0].Walkthrough.WalkthroughName.Equals("any walkthrough name"));
            Assert.IsTrue(resultValue[1].Id.Equals(2));
            Assert.IsTrue(resultValue[1].QuestionText.Equals("any other question"));
            Assert.IsTrue(resultValue[1].Walkthrough.Id.Equals(2));
            Assert.IsTrue(resultValue[1].Walkthrough.WalkthroughName.Equals("any other walkthrough name"));
        }

        [Test]
        public async Task Return_not_found_message_if_no_questions_exist()
        {
            var context = BuildEmptyContext("Questions controller tests empty");
            var getQuestions = new GetQuestions(context);
            var questionController = new QuestionsController(getQuestions, _createQuestion, _deleteQuestion, _updateQuestion);

            var sut = await questionController.GetAll();
            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.NotFound, (sut as ObjectResult).StatusCode);
            Assert.AreEqual("No questions were found", (sut as ObjectResult).Value);
        }

        [TestCase(1, "any question", 1, "any walkthrough name")]
        [TestCase(2, "any other question", 2, "any other walkthrough name")]
        public async Task Return_question_for_walkthrough_when_question_id_passed(int questionId, string question, int walkthroughId, string walkthrough)
        {
            var sut = await _questionsController.GetQuestionsByQuestionId(questionId);

            var resultValue = (sut as OkObjectResult).Value as Question;
            Assert.IsNotNull(resultValue);
            Assert.IsTrue(resultValue.Id.Equals(questionId));
            Assert.IsTrue(resultValue.QuestionText.Equals(question));
            Assert.IsTrue(resultValue.Walkthrough.Id.Equals(walkthroughId));
            Assert.IsTrue(resultValue.Walkthrough.WalkthroughName.Equals(walkthrough));
        }

        [Test]
        public async Task Return_not_found_message_if_no_questions_returned_and_question_id_was_passed_through()
        {
            var sut = await _questionsController.GetQuestionsByQuestionId(3);
            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.NotFound, (sut as ObjectResult).StatusCode);
            Assert.AreEqual("No question was found with this Id", (sut as ObjectResult).Value);
        }

        [TestCase(1, "any question", 1, "any walkthrough name")]
        [TestCase(2, "any other question", 2, "any other walkthrough name")]
        public async Task Return_all_questions_for_walkthrough_when_walkthrough_id_passed(int questionId, string question, int walkthroughId, string walkthrough)
        {
            var sut = await _questionsController.GetQuestionsByWalkthroughId(walkthroughId);

            var resultValue = (sut as OkObjectResult).Value as List<Question>;
            Assert.IsNotNull(resultValue);
            Assert.IsTrue(resultValue.Count.Equals(1));
            Assert.IsTrue(resultValue[0].Id.Equals(questionId));
            Assert.IsTrue(resultValue[0].QuestionText.Equals(question));
            Assert.IsTrue(resultValue[0].Walkthrough.Id.Equals(walkthroughId));
            Assert.IsTrue(resultValue[0].Walkthrough.WalkthroughName.Equals(walkthrough));
        }

        [Test]
        public async Task Return_not_found_message_if_no_questions_exist_for_a_walkthrough()
        {
            var sut = await _questionsController.GetQuestionsByWalkthroughId(3);
            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.NotFound, (sut as ObjectResult).StatusCode);
            Assert.AreEqual("No questions were found for this walkthrough", (sut as ObjectResult).Value);
        }

        [TestCase("any new question", 1, "any walkthrough name")]
        [TestCase("any other new question", 2, "any other walkthrough name")]
        public async Task Return_created_message_when_new_question_created(string questionTest, int walkthroughId, string expectedWalkthroughName)
        {
            var newQuestion = new NewQuestion
            {
                QuestionText = questionTest,
                WalkthroughId = walkthroughId
            };

            var sut = await _questionsController.AddNewQuestion(newQuestion);
            var result = sut as CreatedResult;
            var question = result.Value as Question;

            var questions = await _questionsController.GetAll();
            var questionCount = (questions as OkObjectResult).Value as List<Question>;
            
            Assert.AreEqual(questionTest, question.QuestionText);
            Assert.AreEqual(expectedWalkthroughName, question.Walkthrough.WalkthroughName);
            Assert.AreEqual(3, questionCount.Count);
        }

        [Test]
        public async Task Return_no_content_when_question_has_been_deleted()
        {
            var deletedQuestion = 1;
            var sut = await _questionsController.DeleteQuestion(deletedQuestion);

            var questions = await _questionsController.GetAll();
            var questionCount = (questions as OkObjectResult).Value as List<Question>;

            Assert.IsInstanceOf<NoContentResult>(sut);
            Assert.AreEqual(1, questionCount.Count);
        }

        [Test]
        public async Task Return_question_not_found_when_question_to_be_deleted_does_not_exist()
        {
            var deletedQuestion = 4;
            var sut = await _questionsController.DeleteQuestion(deletedQuestion);

            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual("Question does not exist", (sut as NotFoundObjectResult).Value);
        }

        [Test]
        public async Task Return_question_with_updated_text_after_a_change_has_been_made()
        {
            var updatedQuestion = new Question
            {
                Id = 1,
                QuestionText = "updated question text"
            };
            var sut = await _questionsController.UpdateQuestion(updatedQuestion);

            var question = await _questionsController.GetQuestionsByQuestionId(1);

            Assert.IsInstanceOf<OkObjectResult>(sut);
            Assert.AreEqual("updated question text", ((question as OkObjectResult).Value as Question).QuestionText);
        }

        [Test]
        public async Task Return_question_not_found_when_question_to_be_updated_does_not_exist()
        {
            var updatedQuestion = new Question
            {
                Id = 4,
                QuestionText = "updated question text"
            };
            var sut = await _questionsController.UpdateQuestion(updatedQuestion);

            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual("Question does not exist", (sut as NotFoundObjectResult).Value);
        }

        private DataContext BuildEmptyContext(string key)
        {
            var builder = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(key);
            var context = new DataContext(builder.Options);

            return context;
        }

        private DataContext BuildContext(string key)
        {
            var builder = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(key);
            var context = new DataContext(builder.Options);
            context.Questions.AddRange(new[]
            {
                new Question(){ Id = 1, QuestionText = "any question", Walkthrough = new Walkthrough{Id = 1, WalkthroughName = "any walkthrough name"}},
                new Question(){ Id = 2, QuestionText = "any other question", Walkthrough = new Walkthrough{Id = 2, WalkthroughName = "any other walkthrough name"}}
            });
            context.SaveChanges();

            return context;
        }
    }
}
