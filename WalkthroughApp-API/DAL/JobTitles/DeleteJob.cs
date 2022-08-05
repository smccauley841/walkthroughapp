using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.JobTitles
{
    public class DeleteJob : IDelete<JobTitle>
    {
        private readonly DataContext _context;

        public DeleteJob(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> Delete(JobTitle deletedItem)
        {
            _context.JobTitles.Remove(deletedItem);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
