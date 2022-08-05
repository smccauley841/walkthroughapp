namespace WalkthroughApp_API.DAL
{
    public interface IDelete<T>
    {
        Task<bool> Delete(T deletedItem);
    }
}
