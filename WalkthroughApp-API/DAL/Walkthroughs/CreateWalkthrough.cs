using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Walkthroughs
{
    public class CreateWalkthrough : ICreate<Walkthrough, NewWalkthrough>
    {
        private readonly DataContext _context;

        public CreateWalkthrough(DataContext context)
        {
            _context = context;
        }
        public Walkthrough Create(NewWalkthrough newWalkthrough)
        {
            var employeeRole = _context.JobTitles.FirstOrDefault(x => x.Id == newWalkthrough.EmployeeRole);
            var walkthrough = new Walkthrough
            {
                WalkthroughName = newWalkthrough.WalkthroughName,
                EmployeeRole = employeeRole
            };

            _context.Walkthroughs.Add(walkthrough);

            _context.SaveChanges();

            return walkthrough;
        }

        public bool DoesItemExist(NewWalkthrough newItem)  => _context.Walkthroughs.Any(x =>
            x.WalkthroughName.ToLower() == newItem.WalkthroughName.ToLower() &&
            x.EmployeeRole.Id == newItem.EmployeeRole);
        
    }
}
