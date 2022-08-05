namespace WalkthroughApp_API.Entities
{
    public class Choice
    {
        public int Id { get; set; }
        public string ChoiceText { get; set; }
        public Question QuestionId { get; set; }
        public bool IsAnswer { get; set; }
    }
}
