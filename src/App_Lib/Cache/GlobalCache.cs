using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace src.App_Lib.Cache
{
    public class GlobalCache
    {
        private readonly IMemoryCache _memoryCache;
        private static MemoryCacheEntryOptions? _cacheOptions;

        public GlobalCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;

            _cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(120),
                Priority = CacheItemPriority.Normal,
                SlidingExpiration = new TimeSpan(0, 20, 0)
            };
        }

        public void SetData<T>(string CacheKey, T data)
        {
            _memoryCache.Set(CacheKey, data, _cacheOptions);
        }

        public T GetData<T>(string CacheKey)
        {
            T cacheData;

            if (!_memoryCache.TryGetValue(CacheKey, out cacheData))
            {
                return default;
            }

            return cacheData;
        }
    }
}

/*
 
 [Route("api/[controller]")]
[ApiController]
public class CacheController : ControllerBase
{
    private readonly IMemoryCache memoryCache;
    public CacheController(IMemoryCache memoryCache)
    {
        this.memoryCache = memoryCache;
    }
    [HttpGet("{key}")]
    public IActionResult GetCache(string key)
    {
        string value = string.Empty;
        memoryCache.TryGetValue(key, out value);
        return Ok(value);
    }
    [HttpPost]
    public IActionResult SetCache(CacheRequest data)
    {
        var cacheExpiryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = DateTime.Now.AddMinutes(5),
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromMinutes(2),
            Size = 1024,
        };
        memoryCache.Set(data.key, data.value, cacheExpiryOptions);
        return Ok();
    }
    public class CacheRequest
    {
        public string key { get; set; }
        public string value { get; set; }
    }
}
 
 */