using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Choices
{
    public class DeleteChoices : IDelete<Choice>
    {
        private readonly DataContext _context;

        public DeleteChoices(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> Delete(Choice deletedItem)
        {
            _context.Choices.Remove(deletedItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
