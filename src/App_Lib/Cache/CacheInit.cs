using Microsoft.Extensions.Caching.Memory;

namespace src.App_Lib.Cache;

public static class CacheInit
{
	private static IMemoryCache? _memCache;
	private static MemoryCacheEntryOptions? _cacheOptions;
	
	public static void Configure(IMemoryCache memCache)
	{
		_memCache = memCache;

		_cacheOptions = new MemoryCacheEntryOptions
		{
			AbsoluteExpiration = DateTime.Now.AddMinutes(30),
			Priority = CacheItemPriority.Normal
		};

		StaticCaches();
	}

	public static async void StaticCaches()
	{
		await new _CacheStates(_memCache!).GetData();
		await new _CacheCities(_memCache!).GetData();
	}
}