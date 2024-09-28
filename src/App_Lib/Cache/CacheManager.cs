using Microsoft.Extensions.Caching.Memory;

namespace src.App_Lib.Cache;

public class CacheManager
{
	private readonly IMemoryCache _memoryCache;
	private static MemoryCacheEntryOptions? _cacheOptions;

	public CacheManager(IMemoryCache memoryCache)
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

	public T? GetData<T>(string CacheKey)
	{
		T? cacheData;

		if (!_memoryCache.TryGetValue(CacheKey, out cacheData))
		{
			return default;
		}

		return cacheData;
	}
}