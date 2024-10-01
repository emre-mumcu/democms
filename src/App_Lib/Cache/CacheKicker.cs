using Microsoft.Extensions.Caching.Memory;

namespace src.App_Lib.Cache;

public sealed class CacheKicker
{
	private static readonly Lazy<CacheKicker> lazy = new Lazy<CacheKicker>(() => new CacheKicker());

	public static CacheKicker Instance { get { return lazy.Value; } }

	private CacheKicker() { }

	//
	// **
	//

	private IMemoryCache? _memCache;

	public CacheKicker Configure(IMemoryCache memCache)
	{
		_memCache = memCache;
		return this;
	}

	public async void InitCachesAsync()
	{
		await new _CacheStates(_memCache!).GetCache();
		await new _CacheCities(_memCache!).GetCache();
	}
}