using WalkthroughApp_API.Data;
using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL
{
    public interface ICreate<TOne,TTwo>
    {
        TOne Create(TTwo newItem);

        bool DoesItemExist(TTwo newItem);
    }
}
