namespace WalkthroughApp_API.Entities
{
    public class NewUserQuizResult
    {
        public int userId { get; set; }
        public UserAnswer[] Answers { get; set; }
        public int WalkthroughId { get; set; }

    }
}
