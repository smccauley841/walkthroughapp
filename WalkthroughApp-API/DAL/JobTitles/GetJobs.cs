using Microsoft.EntityFrameworkCore;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.JobTitles
{
    public class GetJobs : IGet<JobTitle>
    {
        private readonly DataContext _context;

        public GetJobs(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobTitle>> GetAll() => await _context.JobTitles.ToListAsync();
        

        public JobTitle GetById(int id) => _context.JobTitles.FirstOrDefault(x => x.Id == id);

        public IList<JobTitle> GetByWalkthroughId(int walkThroughId)
        {
            throw new NotImplementedException();
        }
    }
}
