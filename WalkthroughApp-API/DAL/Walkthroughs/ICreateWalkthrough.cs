using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL.Walkthroughs
{
    public interface ICreateWalkthrough : ICreate<Walkthrough, NewWalkthrough>
    {
        bool DoesWalkthroughExist(NewWalkthrough newWalkthrough);
    }
}
