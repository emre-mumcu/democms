using Microsoft.Extensions.Caching.Memory;
using src.App_Lib.Configuration;

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
			AbsoluteExpiration = Literals.Cache_Absolute_Expiration,
			SlidingExpiration = Literals.Cache_SlidingExpiration_Timespan,
			Priority = Literals.Cache_Priority
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