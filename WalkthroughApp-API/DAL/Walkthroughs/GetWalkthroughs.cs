using Microsoft.EntityFrameworkCore;
using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Walkthroughs
{
    public class GetWalkthroughs : IGet<Walkthrough>
    {
        private readonly DataContext _context;

        public GetWalkthroughs(DataContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Walkthrough>> GetAll() => _context.Walkthroughs.ToList();

        public Walkthrough GetById(int id) => _context.Walkthroughs.FirstOrDefault(x => x.Id == id);
        
    }
}
