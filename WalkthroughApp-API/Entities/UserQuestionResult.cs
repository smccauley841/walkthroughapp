namespace WalkthroughApp_API.Entities;

public class UserQuestionResult
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public Question QuestionId { get; set; }
    public Choice SelectedChoice { get; set; }
    public bool IsAnswerCorrect { get; set; }
    public UserWalkthroughQuiz QuizId { get; set; }

}