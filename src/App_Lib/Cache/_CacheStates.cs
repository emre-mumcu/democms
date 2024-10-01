using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using src.App_Data;
using src.App_Data.LookUps;

namespace src.App_Lib.Cache
{
    public class _CacheStates : CacheBase<State>
    {
        public _CacheStates(IMemoryCache memCache) : base(memCache) { }
        private async Task<List<State>> SetCache() => await new AppDbContext().States.ToListAsync();
        public async Task<List<State>?> GetCache(bool isDirty = false) => await GetCache(SetCache, isDirty);
    }
}