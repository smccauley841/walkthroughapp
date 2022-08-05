using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Walkthroughs
{
    public class UpdateWalkthrough : IUpdate<Walkthrough>
    {
        private readonly DataContext _context;

        public UpdateWalkthrough(DataContext context)
        {
            _context = context;
        }
        public async Task<Walkthrough> UpdateItem(Walkthrough item)
        {
            _context.Walkthroughs.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
