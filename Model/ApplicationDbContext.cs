﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using targeteo.pl.Model.Entities;

namespace targeteo.pl.Model
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AccountOperationStatus> AccountOperationStatus { get; set; }
        public virtual DbSet<Adress> Adress { get; set; }
        public virtual DbSet<AnnouncementPayment> AnnouncementPayment { get; set; }
        public virtual DbSet<AnnouncementStatus> AnnouncementStatus { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AttributeValueList> AttributeValueList { get; set; }
        public virtual DbSet<Cart> Cart { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<CategoryAttributes> CategoryAttributes { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Comments> Comments { get; set; }
        public virtual DbSet<CompanyInformation> CompanyInformation { get; set; }
        public virtual DbSet<Conversations> Conversations { get; set; }
        public virtual DbSet<Discounts> Discounts { get; set; }
        public virtual DbSet<DiscountsProducts> DiscountsProducts { get; set; }
        public virtual DbSet<MigrationHistory> MigrationHistory { get; set; }
        public virtual DbSet<OneUserOrder> OneUserOrder { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<OrderOpinion> OrderOpinion { get; set; }
        public virtual DbSet<OrderSummary> OrderSummary { get; set; }
        public virtual DbSet<Picture> Picture { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttribute { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<SupplyMethodType> SupplyMethodType { get; set; }
        public virtual DbSet<SupplyMethods> SupplyMethods { get; set; }
        public virtual DbSet<UserAccount> UserAccount { get; set; }
        public virtual DbSet<UserProfile> UserProfile { get; set; }
        public virtual DbSet<UserProfileSettings> UserProfileSettings { get; set; }
        public virtual DbSet<UserRoles> UserRoles { get; set; }
        public virtual DbSet<UserSupplyMethod> UserSupplyMethod { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        [DbFunction("udf_Haversine", "dbo")]
        public double udf_Haversine(double cityLat, double cityLon, double lat, double lon)
        {
            throw new NotImplementedException();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountOperationStatus>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Caption)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Adress>(entity =>
            {
                entity.Property(e => e.Cellphone)
                    .IsRequired()
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<AnnouncementPayment>(entity =>
            {
                entity.Property(e => e.DateFrom).HasColumnType("datetime");

                entity.Property(e => e.DateTo).HasColumnType("datetime");

                entity.Property(e => e.Link).IsRequired();

                entity.Property(e => e.PaymentId)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<AnnouncementStatus>(entity =>
            {
                entity.Property(e => e.CaptionPl)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.AspNetUserClaims_dbo.AspNetUsers_UserId");
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey, e.UserId })
                    .HasName("PK_dbo.AspNetUserLogins");

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.AspNetUserLogins_dbo.AspNetUsers_UserId");
            });

            modelBuilder.Entity<AttributeValueList>(entity =>
            {
                entity.Property(e => e.FkCategoryAttrId).HasColumnName("FK_CategoryAttrId");

                entity.Property(e => e.FkProductAttrId).HasColumnName("FK_ProductAttrId");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.RecordId);

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Cart)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cart_ToProduct");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Caption)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Icon)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.ThumbnailFileName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Category_ToCategory");
            });

            modelBuilder.Entity<CategoryAttributes>(entity =>
            {
                entity.Property(e => e.AttributeType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ViewFilterType)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.AttributeValueList)
                    .WithMany(p => p.CategoryAttributes)
                    .HasForeignKey(d => d.AttributeValueListId)
                    .HasConstraintName("FK_CategoryAttributes_AttributeValueList");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.CategoryAttributes)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CategoryAttributes_Category");
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.City1)
                    .IsRequired()
                    .HasColumnName("City")
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Comments>(entity =>
            {
                entity.Property(e => e.Content)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UserIdFrom)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.UserIdTo)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.Conversation)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ConversationId)
                    .HasConstraintName("FK_Comments_Conversations");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comments_Product");

                entity.HasOne(d => d.UserIdFromNavigation)
                    .WithMany(p => p.CommentsUserIdFromNavigation)
                    .HasForeignKey(d => d.UserIdFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comments_AspNetUsers_UserFrom");

                entity.HasOne(d => d.UserIdToNavigation)
                    .WithMany(p => p.CommentsUserIdToNavigation)
                    .HasForeignKey(d => d.UserIdTo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Comments_AspNetUsers_UserTo");
            });

            modelBuilder.Entity<CompanyInformation>(entity =>
            {
                entity.Property(e => e.Address).IsRequired();

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description).IsRequired();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.EmailPassword)
                    .IsRequired()
                    .HasColumnName("emailPassword")
                    .HasMaxLength(50);

                entity.Property(e => e.Logo)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Pop3Server)
                    .HasColumnName("pop3Server")
                    .HasMaxLength(200);

                entity.Property(e => e.Port).HasColumnName("port");

                entity.Property(e => e.PostalCode)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.SmtpServer)
                    .IsRequired()
                    .HasColumnName("smtpServer")
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Conversations>(entity =>
            {
                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Conversations)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Conversations_Product");
            });

            modelBuilder.Entity<Discounts>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<DiscountsProducts>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IdProduct)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.DiscountsProducts)
                    .HasForeignKey<DiscountsProducts>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiscountsProducts_Discounts");

                entity.HasOne(d => d.IdProductNavigation)
                    .WithMany(p => p.DiscountsProducts)
                    .HasForeignKey(d => d.IdProduct)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DiscountsProducts_Product");
            });

            modelBuilder.Entity<MigrationHistory>(entity =>
            {
                entity.HasKey(e => new { e.MigrationId, e.ContextKey })
                    .HasName("PK_dbo.__MigrationHistory");

                entity.ToTable("__MigrationHistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ContextKey).HasMaxLength(300);

                entity.Property(e => e.Model).IsRequired();

                entity.Property(e => e.ProductVersion)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<OneUserOrder>(entity =>
            {
                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.AspNetUserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.OrderSummaryId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.PaymentUrl).IsUnicode(false);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.PostalCode)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TrackingNo)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.AspNetUser)
                    .WithMany(p => p.OneUserOrder)
                    .HasForeignKey(d => d.AspNetUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Order_AspNetUsers");

                entity.HasOne(d => d.OrderSummary)
                    .WithMany(p => p.OneUserOrder)
                    .HasForeignKey(d => d.OrderSummaryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OneUserOrder_OrderSummary");

                entity.HasOne(d => d.UserSupplyMethod)
                    .WithMany(p => p.OneUserOrder)
                    .HasForeignKey(d => d.UserSupplyMethodId)
                    .HasConstraintName("FK_OneUserOrder_UserSupplyMethod");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.OneUserOrder)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.OneUserOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_ToOneUserOrder");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetail)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_ToProduct");
            });

            modelBuilder.Entity<OrderOpinion>(entity =>
            {
                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasMaxLength(350)
                    .IsUnicode(false);

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.OrderOpinion)
                    .HasForeignKey(d => d.OrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetail_OrderOpinion");

                entity.HasOne(d => d.Picture)
                    .WithMany(p => p.OrderOpinion)
                    .HasForeignKey(d => d.PictureId)
                    .HasConstraintName("FK_PictureId_Pictures");
            });

            modelBuilder.Entity<OrderSummary>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.PaymentUrl).IsUnicode(false);
            });

            modelBuilder.Entity<Picture>(entity =>
            {
                entity.Property(e => e.Path)
                    .IsRequired()
                    .HasMaxLength(350)
                    .IsUnicode(false);

                entity.Property(e => e.ProdId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.Prod)
                    .WithMany(p => p.Picture)
                    .HasForeignKey(d => d.ProdId)
                    .HasConstraintName("FK_Picture_Product");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.AspNetUserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.CreationDate).HasColumnType("datetime");

                entity.Property(e => e.DateTo).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1500)
                    .IsUnicode(false);

                entity.Property(e => e.ThumbPath)
                    .IsRequired()
                    .HasMaxLength(350)
                    .IsUnicode(false);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.AspNetUser)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.AspNetUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_AspNetUsers");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Category");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_City");
            });

            modelBuilder.Entity<ProductAttribute>(entity =>
            {
                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.CategoryAttribute)
                    .WithMany(p => p.ProductAttribute)
                    .HasForeignKey(d => d.CategoryAttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductAttribute_CategoryAttribute");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductAttribute)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProductAttribute_Product");
            });

            modelBuilder.Entity<Roles>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<SupplyMethodType>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SupplyMethods>(entity =>
            {
                entity.Property(e => e.Company)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.LogoPath)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.SpecificDeliveryMethod)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.HasOne(d => d.SupplyMethodType)
                    .WithMany(p => p.SupplyMethods)
                    .HasForeignKey(d => d.SupplyMethodTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SupplyMethods_SupplyMethodType");
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.Property(e => e.AccountNo)
                    .IsRequired()
                    .HasMaxLength(32);

                entity.Property(e => e.OperationAmount).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.OperationDate).HasColumnType("date");

                entity.Property(e => e.OrderSummaryId)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.OrderSummary)
                    .WithMany(p => p.UserAccount)
                    .HasForeignKey(d => d.OrderSummaryId)
                    .HasConstraintName("FK_UserAccount_OrderSummary");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.UserAccount)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAccount_OperationStatus");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAccount)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserAccount_User");
            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.Property(e => e.AcountNo)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId })
                    .HasName("PK_dbo.AspNetUserRoles");

                entity.Property(e => e.UserId).HasMaxLength(128);

                entity.Property(e => e.RoleId).HasMaxLength(128);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRoles)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId");
            });

            modelBuilder.Entity<UserSupplyMethod>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.SupplyMethod)
                    .WithMany(p => p.UserSupplyMethod)
                    .HasForeignKey(d => d.SupplyMethodId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSupplyMethod_SupplyMethod");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserSupplyMethod)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSupplyMethod_User");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.Id).HasMaxLength(128);

                entity.Property(e => e.BannEndDate).HasColumnType("datetime2(2)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.LockoutEndDateUtc).HasColumnType("datetime2(2)");

                entity.Property(e => e.RefreshToken).HasMaxLength(256);

                entity.Property(e => e.RegisterDate).HasColumnType("datetime2(2)");

                entity.Property(e => e.TokenExpirationDateTime).HasColumnType("datetime2(2)");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.HasOne(d => d.Adress)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.AdressId)
                    .HasConstraintName("FK_AspNetUsers_Adress");

                entity.HasOne(d => d.UserProfile)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserProfileId)
                    .HasConstraintName("FK_AspNetUsers_UserProfile");

                entity.HasOne(d => d.UserProfileSettings)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.UserProfileSettingsId)
                    .HasConstraintName("FK_dbo.AspNetUsers_dbo.UserProfileSettings_UserProfileSettingsId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}