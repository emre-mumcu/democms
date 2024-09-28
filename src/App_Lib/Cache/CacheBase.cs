using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Caching.Memory;
using src.App_Data.Types;
using src.App_Lib.Extensions;

namespace src.App_Lib.Cache;

public class CacheBase<T> where T : new()
{
    private readonly IMemoryCache? _memCache;
    private readonly MemoryCacheEntryOptions? _cacheOptions;

    public CacheBase(IMemoryCache memCache)
    {
        _memCache = memCache;

        _cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(60),
            SlidingExpiration = new TimeSpan(0, 20, 0),
            Priority = CacheItemPriority.Normal
        };
    }

    protected async Task<List<T>> GetCachedData(string cacheName, Func<Task<List<T>>> fillCacheData, bool isDirty = false)
    {
        

        try
        {
            

            if (isDirty) _memCache.Remove(cacheName);

            List<T> data;

            if (!_memCache.TryGetValue(cacheName, out data))
            {
                data = await Task.Run(fillCacheData);
                _memCache.Set(cacheName, data, _cacheOptions);
            }

            return data;
        }
        catch (Exception ex)
        {
            
            return default;
        }
    }
}

