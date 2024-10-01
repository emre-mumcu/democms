using Microsoft.Extensions.Caching.Memory;
using src.App_Lib.Configuration;

namespace src.App_Lib.Cache;

public abstract class CacheBase<T> /* where T : new() */ // In order to use required parameters in T; new() constraint is removed
{
	private readonly IMemoryCache _memCache;
	private readonly MemoryCacheEntryOptions _cacheOptions;

	public CacheBase(IMemoryCache memCache)
	{
		_memCache = memCache;

		_cacheOptions = new MemoryCacheEntryOptions
		{
			AbsoluteExpiration = Literals.Cache_Absolute_Expiration,
			SlidingExpiration = Literals.Cache_SlidingExpiration_Timespan,
			Priority = Literals.Cache_Priority
		};
	}

	protected async Task<List<T>?> GetCache(Func<Task<List<T>>> SetCache, bool isDirty = false)
	{
		try
		{
			if (isDirty || !_memCache.TryGetValue(nameof(T), out List<T>? data))
			{				
				data = await Task.Run(SetCache);
				_memCache.Set(nameof(T), data, _cacheOptions);
			}

			return data;
		}
		catch
		{
			return default;
		}
	}
}