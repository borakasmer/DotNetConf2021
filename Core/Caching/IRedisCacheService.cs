using System;
using System.Collections.Generic;
using System.Text;

namespace Dashboard.Core.Caching
{
    /*
        Projede Redis kütüphanesi olarak servicestack kullanıyoruz. 
        Limit: The free-quota limit on '6000 Redis requests per hour' has been reached (Lisansız versiyon için)
    */
    public interface IRedisCacheService
    {
        T Get<T>(string key);
        IList<T> GetAll<T>(string key);
        void Set(string key, object data);
        void Set(string key, object data, DateTime time);
        void SetAll<T>(IDictionary<string, T> values);
        bool IsSet(string key);
        void Remove(string key);
        void RemoveByPattern(string pattern);
        void Clear();
        int Count(string key);           
    }
}
