using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Choices
{
    public interface IGetChoices : IGet<Choice>
    {
        Task<IList<Choice>> GetByQuestionId(int questionId);
    }
}
