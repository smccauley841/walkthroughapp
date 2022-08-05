using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Questions
{
    public class UpdateQuestion : IUpdate<Question>
    {
        private readonly DataContext _context;

        public UpdateQuestion(DataContext context)
        {
            _context = context;
        }
        public async Task<Question> UpdateItem(Question item)
        {
            _context.Questions.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
