using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.App_Lib.Attributes
{
    public class RoleDescriptionAttribute : Attribute
    {
        public bool Restricted { get; set; }
        public string RoleName { get; set; }
        public string RoleDetail { get; set; }
    }
}