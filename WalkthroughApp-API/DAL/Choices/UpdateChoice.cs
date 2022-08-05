using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Choices
{
    public class UpdateChoice : IUpdate<Choice>
    {
        private readonly DataContext _context;

        public UpdateChoice(DataContext context)
        {
            _context = context;
            _context = context;
        }
        public async Task<Choice> UpdateItem(Choice item)
        {
            _context.Choices.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
