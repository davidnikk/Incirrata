using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Incirrata.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Role { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public List<RoleViewModel> Roles { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}