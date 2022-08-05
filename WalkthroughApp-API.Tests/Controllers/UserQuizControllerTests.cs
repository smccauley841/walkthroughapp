using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using WalkthroughApp_API.Controllers;
using WalkthroughApp_API.DAL.Choices;
using WalkthroughApp_API.DAL.UserQuiz;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;
using WalkthroughApp_API.Tests.BuildTestDatabase;

namespace WalkthroughApp_API.Tests.Controllers
{
    public class UserQuizControllerTests
    {

        private CreateTestDatabase _createTestDatabase;
        private DataContext _context;
        private IGetUserQuizzes _getUserQuizzes;
        private UserQuizController _userQuizController;

        [SetUp]
        public void Setup()
        {
            _createTestDatabase = new CreateTestDatabase();
            _context = _createTestDatabase.BuildContext("User quizzes controller tests");
            _getUserQuizzes = new GetUserQuizzes(_context);
            _userQuizController = new UserQuizController(_getUserQuizzes);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
        }

        [Test]
        public async Task Return_all_walkthrough_quizzes_completed_by_the_user()
        {
            var sut = await _userQuizController.GetWalkthroughsWithCompletedQuizByUserId(1);

            var resultValue = (sut as OkObjectResult).Value as IList<Walkthrough>;
            Assert.IsNotNull(resultValue);
            Assert.AreEqual(1, resultValue.Count());
        }

        [Test]
        public async Task Return_not_found_message_if_no_walkthrough_quizzes_were_completed_by_the_user()
        {
            var sut = await _userQuizController.GetWalkthroughsWithCompletedQuizByUserId(4);

            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.NotFound, (sut as ObjectResult).StatusCode);
            Assert.AreEqual("No completed walkthrough quizzes for this user", (sut as ObjectResult).Value);
        }

        [Test]
        public async Task Return_all_walkthrough_quiz_instances_completed_by_the_user()
        {
            var sut = await _userQuizController.GetCompletedQuizzesForWalkthroughByUserId(1, 1);

            var resultValue = (sut as OkObjectResult).Value as IList<UserWalkthroughQuiz>;
            Assert.IsNotNull(resultValue);
            Assert.AreEqual(3, resultValue.Count());
        }

        [Test]
        public async Task Return_not_found_message_if_no_walkthrough_quizzes_were_completed_by_the_user_when_searching_by_walkthrough_and_user()
        {
            var sut = await _userQuizController.GetCompletedQuizzesForWalkthroughByUserId(1, 4);

            Assert.IsInstanceOf<NotFoundObjectResult>(sut);
            Assert.AreEqual((int)HttpStatusCode.NotFound, (sut as ObjectResult).StatusCode);
            Assert.AreEqual("No completed quizzes for this user in this walkthrough", (sut as ObjectResult).Value);
        }

        [Test]
        public async Task Return_all_question_results_when_searching_by_quiz_id()
        {
            var sut = await _userQuizController.GetUserAnswersForAQuiz(1);

            var resultValue = (sut as OkObjectResult).Value as IList<UserWalkthroughQuiz>;
            Assert.IsNotNull(resultValue);
            Assert.AreEqual(3, resultValue.Count());
        }

        [Test]
        public async Task Return_created_message_when_user_has_completed_a_quiz_with_records_created_in_quiz_tables()
        {
            var userAnswer = new NewUserQuizResult
            {
                userId = 1,
                WalkthroughId = 1,
                Answers = new UserAnswer[]
                {
                    new UserAnswer{ QuestionId = 3, UserChoiceId = 10},
                    new UserAnswer{ QuestionId = 1, UserChoiceId = 1}
                }
            };
            var sut = await _userQuizController.AddNewQuizEntry(userAnswer);
            var userWalkThroughQuiz = await _userQuizController.GetCompletedQuizzesForWalkthroughByUserId(1, 1);

            Assert.AreEqual(3, ((sut as OkObjectResult).Value as IList<UserWalkthroughQuiz>).Count);
        }

    }
}
