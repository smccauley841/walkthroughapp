using Microsoft.EntityFrameworkCore;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Walkthrough> Walkthroughs { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<UserWalkthroughQuiz> UserWalkthroughQuizzes { get; set; }
        public DbSet<UserQuestionResult> UserQuestionResults { get; set; }
    }
}
