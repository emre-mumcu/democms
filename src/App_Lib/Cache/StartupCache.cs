using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using src.App_Data;
using src.App_Data.Entities;
using src.App_Lib.Tools;

namespace src.App_Lib.Cache
{
    public static class StartupCache
    {
        private static IMemoryCache? _memCache;
        private static MemoryCacheEntryOptions? _cacheOptions;
        private static ManagerTools _managerTools;
        public static void StartupCacheConfig(IMemoryCache memCache)
        {
            _memCache = memCache;

            _cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(30),
                Priority = CacheItemPriority.Normal
            };

        }

        public static async Task<List<UserRoleRight>> GetDbYetkiler(string? roleCode = null, bool refresh = false)
        {
            string itemKeyName = "DbYetkiler";

            if (refresh) _memCache!.Remove(itemKeyName);

            List<UserRoleRight> list;

            if (!_memCache.TryGetValue(itemKeyName, out list))
            {
                using (var context = new AppDbContext())
                {
                    list = await context.UserRoleRights
                        .ToListAsync();
                }

                _memCache.Set(itemKeyName, list, _cacheOptions);
            }

            if (roleCode != null) return list.Where(i => i.RoleCode == roleCode).ToList();
            else return list;
        }
    }
}