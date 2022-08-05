using System.Collections;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.UserQuiz
{
    public interface IGetUserQuizzes
    {
        Task<IEnumerable<Walkthrough>> GetWalkthroughWithCompletedQuizzesByUserId(int userId);
        Task<IEnumerable<UserWalkthroughQuiz>> GetUserCompletedWalkthroughQuizzesByWalkthroughId(int userId, int walkthroughId);
        Task<IEnumerable<UserQuestionResult>> GetUserAnswersForQuiz(int quizId);
    }
}
