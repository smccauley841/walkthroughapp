using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.JobTitles
{
    public class UpdateJob : IUpdate<JobTitle>
    {
        private readonly DataContext _context;

        public UpdateJob(DataContext context)
        {
            _context = context;
        }
        public async Task<JobTitle> UpdateItem(JobTitle item)
        {
            _context.JobTitles.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
