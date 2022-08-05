using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Questions
{
    public interface IGetQuestions : IGet<Question>
    {
        IList<Question> GetByWalkthroughId(int walkThroughId);
    }
}
