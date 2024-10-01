using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using src.App_Data;
using src.App_Data.LookUps;

namespace src.App_Lib.Cache;

public class _CacheCities : CacheBase<City>
{
	public _CacheCities(IMemoryCache memCache) : base(memCache) { }
	private async Task<List<City>> SetCache() => await new AppDbContext().Cities.ToListAsync();
	public async Task<List<City>?> GetCache(bool isDirty = false) => await GetCache(SetCache, isDirty);
}