namespace _102techBot.Domain.Repositories
{
    internal interface ITemporaryStorageRepository
    {
        void Add<T>(string key, T value);
        T? Get<T>(string key);
        bool Remove(string key);
    }
}
