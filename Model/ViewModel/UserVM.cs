using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace targeteo.pl.Model.ViewModel
{
    public class UserVM
    {
        public UserVM()
        {
            Rola = new RolaVM();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool Banned { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? BannEndDate { get; set; }
        public RolaVM Rola { get; set; }

        public ICollection<RolaVM> UserRoles { get; set; }
        public ICollection<UserSupplyMethodVm> UserSupplyMethods { get; set; }
    }
}