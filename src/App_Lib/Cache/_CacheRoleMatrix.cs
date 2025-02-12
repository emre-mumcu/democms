using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using src.App_Data;
using src.App_Data.Entities;

namespace src.App_Lib.Cache;

public class _CacheRoleMatrix : CacheBase<DynamicRole>
{
	public _CacheRoleMatrix(IMemoryCache memCache) : base(memCache) { }
	private async Task<List<DynamicRole>> FillCache() => await new AppDbContext().DynamicRoles.ToListAsync();
	public async Task<List<DynamicRole>?> GetCache(bool isDirty = false) => await GetCache(FillCache, isDirty);
}