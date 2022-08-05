using Microsoft.EntityFrameworkCore;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Choices
{
    public class GetChoices : IGetChoices
    {
        private readonly DataContext _context;

        public GetChoices(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Choice>> GetAll() => await _context.Choices.ToListAsync();

        public Choice GetById(int id) => _context.Choices.FirstOrDefault(x => x.Id == id);
        public async Task<IList<Choice>> GetByQuestionId(int questionId)
        {
            var choices = await _context.Choices.Where(x => x.QuestionId.Id == questionId).ToListAsync();
            return choices;
        }
    }
}
