namespace WalkthroughApp_API.DAL
{
    public interface IUpdate<T>
    {
        Task<T> UpdateItem(T item);
    }
}
