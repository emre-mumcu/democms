using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace src.App_Data.Types
{
    public enum EnumCacheNames
    {
        [Display(Name = "Cache 1")]
        [Description("Cache1")]
        Cache1 = 1,
        [Description("Cache2")]
        Cache2 = 2
    }

    
}