
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Walkthroughs
{
    public class DeleteWalkthrough : IDelete<Walkthrough>
    {
        private readonly DataContext _context;

        public DeleteWalkthrough(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> Delete(Walkthrough deletedWalkthrough)
        {
            _context.Walkthroughs.Remove(deletedWalkthrough);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
