using Microsoft.AspNetCore.Mvc;
using WalkthroughApp_API.DAL.UserQuiz;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.Controllers
{
    public class UserQuizController : WalkthroughAppApiController
    {
        private readonly IGetUserQuizzes _getUserQuizzes;

        public UserQuizController(IGetUserQuizzes getUserQuizzes)
        {
            _getUserQuizzes = getUserQuizzes;
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWalkthroughsWithCompletedQuizByUserId(int userId)
        {
            var walkthroughs = await _getUserQuizzes.GetWalkthroughWithCompletedQuizzesByUserId(userId);
            if(!walkthroughs.Any())
                return NotFound("No completed walkthrough quizzes for this user");
            return Ok(walkthroughs);
        }

        [HttpGet("{userId}/{walkthroughId}")]
        public async Task<IActionResult> GetCompletedQuizzesForWalkthroughByUserId(int userId, int walkThroughId)
        {
            var quizzAttempts =
                await _getUserQuizzes.GetUserCompletedWalkthroughQuizzesByWalkthroughId(userId, walkThroughId);

            if (!quizzAttempts.Any())
                return NotFound("No completed quizzes for this user in this walkthrough");
            return Ok(quizzAttempts);

        }

        [HttpGet("{userQuizId}")]
        public async Task<IActionResult> GetUserAnswersForAQuiz(int userQuizId)
        {
            var quizAnswers =
                await _getUserQuizzes.GetUserAnswersForQuiz(userQuizId);

            if (!quizAnswers.Any())
                return NotFound("Quiz Results do not exist");

            return Ok(quizAnswers);

        }

        [HttpPost]
        public async Task<IActionResult> AddNewQuizEntry([FromBody]NewUserQuizResult userAnswer)
        {
            throw new NotImplementedException();
        }
    }
}
