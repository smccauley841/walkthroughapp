using Microsoft.EntityFrameworkCore;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.UserQuiz
{
    public class GetUserQuizzes : IGetUserQuizzes
    {
        private readonly DataContext _context;

        public GetUserQuizzes(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Walkthrough>> GetWalkthroughWithCompletedQuizzesByUserId(int userId) => await _context.UserWalkthroughQuizzes
                                                                                                                            .Where(x => x.UserId == userId)
                                                                                                                            .Select(x => x.Walkthrough)
                                                                                                                            .Distinct()
                                                                                                                            .ToListAsync();

        public async Task<IEnumerable<UserWalkthroughQuiz>> GetUserCompletedWalkthroughQuizzesByWalkthroughId(int userId, int walkthroughId) => await _context.UserWalkthroughQuizzes
                                                                                                                                                              .Where(x => x.UserId == userId && x.Walkthrough.Id == walkthroughId)
                                                                                                                                                              .ToListAsync();

        public async Task<IEnumerable<UserQuestionResult>> GetUserAnswersForQuiz(int quizId) => await _context.UserQuestionResults
                                                                                                              .Where(x => x.QuizId.Id == quizId)
                                                                                                              .ToListAsync();
    }
}
