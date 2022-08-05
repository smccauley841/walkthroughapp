using WalkthroughApp_API.Entities;

namespace WalkthroughApp_API.DAL
{
    public interface IGet<T>
    {
        Task<IEnumerable<T>> GetAll();
        T GetById(int id);
    }
}
