namespace WalkthroughApp_API.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public Walkthrough Walkthrough { get; set; }
    }
}
