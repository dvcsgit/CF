using System;
using System.Web.Caching;

namespace Utility
{
    public static class CacheHelper
    {
        public static T GetOrInsert<T>(this Cache cache,string key,Func<T> generator)
        {
            var result = cache[key];

            if (result != null)
            {
                return (T)result;
            }
            else
            {
                result = (generator != null) ? generator() : default(T);

                cache.Insert(key, result, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration);

                return (T)result;
            }
        }
    }
}
