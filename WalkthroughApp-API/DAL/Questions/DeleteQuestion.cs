using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Questions
{
    public class DeleteQuestion : IDelete<Question>
    {
        private readonly DataContext _context;
        private readonly IGet<Question> _getQuestions;

        public DeleteQuestion(DataContext context, IGet<Question> getQuestions)
        {
            _context = context;
            _getQuestions = getQuestions;
        }
        public async Task<bool> Delete(Question deletedQuestion)
        {
            _context.Questions.Remove(deletedQuestion);
            await _context.SaveChangesAsync();
            return true;

        }
    }
}
