using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using src.App_Data;
using src.App_Data.Entities;

namespace src.App_Lib.Cache;

public class _CacheRoleMatrix : CacheBase<RoleMatrix>
{
	public _CacheRoleMatrix(IMemoryCache memCache) : base(memCache) { }
	private async Task<List<RoleMatrix>> FillCache() => await new AppDbContext().RoleMatrixes.ToListAsync();
	public async Task<List<RoleMatrix>?> GetCache(bool isDirty = false) => await GetCache(FillCache, isDirty);
}