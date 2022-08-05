using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Choices
{
    public interface ICreateChoices
    {
        Task<IList<Choice>> CreateQuestionChoices(NewChoice[] newChoices);
        Task<bool> DoesQuestionChoiceExist(NewChoice newChoice);
        Task<bool> DoesQuestionAlreadyHaveACorrectAnswer(NewChoice newChoice);
    }
}
