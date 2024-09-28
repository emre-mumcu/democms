using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using src.App_Data;
using src.App_Data.LookUps;
using src.App_Data.Types;

namespace src.App_Lib.Cache
{
    public class _CacheStates : CacheBase<State>
    {
        public _CacheStates(IMemoryCache memCache) : base(memCache) { }
        private async Task<List<State>> FillCache() => await new AppDbContext().States.ToListAsync();
        public async Task<List<State>> GetData(bool isDirty = false) => await GetCachedData(this.GetType().Name, FillCache, isDirty);
    }
}