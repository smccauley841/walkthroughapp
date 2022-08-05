namespace WalkthroughApp_API.Entities
{
    public class UserWalkthroughQuiz
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public Walkthrough Walkthrough { get; set; }
        public DateTime TimeTaken { get; set; }
    }
}
