using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace src.App_Data.Entities
{
    public class UserRoleRight : BaseEntity
    {
        public string RoleCode { get; set; }
        public string FullName { get; set; }
        public string DepartmentId { get; set; }
        public string UserName { get; set; }
        public bool Allow { get; set; } = true;
    }
}