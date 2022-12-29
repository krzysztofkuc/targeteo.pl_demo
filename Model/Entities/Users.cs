﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace targeteo.pl.Model.Entities
{
    public partial class Users
    {
        public Users()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaims>();
            AspNetUserLogins = new HashSet<AspNetUserLogins>();
            CommentsUserIdFromNavigation = new HashSet<Comments>();
            CommentsUserIdToNavigation = new HashSet<Comments>();
            OneUserOrder = new HashSet<OneUserOrder>();
            Product = new HashSet<Product>();
            UserAccount = new HashSet<UserAccount>();
            UserRoles = new HashSet<UserRoles>();
            UserSupplyMethod = new HashSet<UserSupplyMethod>();
        }

        public string Id { get; set; }
        public int? UserProfileSettingsId { get; set; }
        public bool Banned { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime? BannEndDate { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public int? AdressId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? TokenExpirationDateTime { get; set; }
        public int? UserProfileId { get; set; }

        public virtual Adress Adress { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual UserProfileSettings UserProfileSettings { get; set; }
        public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual ICollection<Comments> CommentsUserIdFromNavigation { get; set; }
        public virtual ICollection<Comments> CommentsUserIdToNavigation { get; set; }
        public virtual ICollection<OneUserOrder> OneUserOrder { get; set; }
        public virtual ICollection<Product> Product { get; set; }
        public virtual ICollection<UserAccount> UserAccount { get; set; }
        public virtual ICollection<UserRoles> UserRoles { get; set; }
        public virtual ICollection<UserSupplyMethod> UserSupplyMethod { get; set; }
    }
}