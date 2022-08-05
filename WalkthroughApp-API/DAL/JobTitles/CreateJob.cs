using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.JobTitles
{
    public class CreateJob : ICreate<JobTitle, NewJobTitle>
    {
        private readonly DataContext _context;

        public CreateJob(DataContext context)
        {
            _context = context;
        }
        public JobTitle Create(NewJobTitle newItem)
        {
            var jobTitle = new JobTitle
            {
                Name = newItem.Name
            };

            _context.JobTitles.Add(jobTitle);

            _context.SaveChanges();

            return jobTitle;
        }

        public bool DoesItemExist(NewJobTitle newItem) => _context.JobTitles.Any(x =>
        x.Name.ToLower() == newItem.Name.ToLower());
    }
}
