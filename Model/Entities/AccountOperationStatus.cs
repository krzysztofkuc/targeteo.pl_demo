﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace targeteo.pl.Model.Entities
{
    public partial class AccountOperationStatus
    {
        public AccountOperationStatus()
        {
            UserAccount = new HashSet<UserAccount>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }

        public virtual ICollection<UserAccount> UserAccount { get; set; }
    }
}