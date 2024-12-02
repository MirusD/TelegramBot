using _102techBot.Domain.Repositories;
using System.Collections.Concurrent;

namespace _102techBot.Infrastructure.DAL
{
    public class TemporaryStorageRepository : ITemporaryStorageRepository
    {
        private readonly ConcurrentDictionary<string, object> _storage = new();

        public void Add<T>(string key, T value)
        {
            _storage[key] = value!;
        }

        public T? Get<T>(string key)
        {
            return _storage.TryGetValue(key, out var value) ? (T)value : default;
        }

        public bool Remove(string key)
        {
            return _storage.TryRemove(key, out _);
        }
    }
}
