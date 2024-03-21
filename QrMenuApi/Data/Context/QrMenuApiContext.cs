using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QrMenuApi.Data.Models;

namespace QrMenuApi.Data.Context
{
    public class QrMenuApiContext : IdentityDbContext<ApplicationUser>
    {
        public QrMenuApiContext(DbContextOptions<QrMenuApiContext> options) : base(options)
        {
        }

        public DbSet<State>? States { get; set; }
        public DbSet<Company>? Companies { get; set; }
        public DbSet<Restaurant>? Restaurants { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Food>? Foods { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region ModelConfigurations

            //State
            modelBuilder.Entity<State>(entity =>
             {
                 entity.Property(s => s.Name).IsRequired().HasMaxLength(20);
             });

            //Company
            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(c => c.Name).IsRequired().HasColumnType("nvarchar(200)");
                entity.Property(c => c.PostalCode).IsRequired().HasColumnType("char(5)");
                entity.Property(c => c.Address).IsRequired().HasColumnType("nvarchar(200)");
                entity.Property(c => c.PhoneNumber).IsRequired().HasColumnType("varchar(30)");
                entity.Property(c => c.EMail).IsRequired().HasColumnType("varchar(50)");
                entity.Property(c => c.TaxNumber).IsRequired().HasColumnType("varchar(11)");
                entity.Property(c => c.WebAddress).IsRequired().HasColumnType("varchar(100)");
                entity.Property(c => c.RegisterDate).IsRequired().HasColumnType("smalldatetime");

            });

            //Restaurant
            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.Property(r => r.Name).IsRequired().HasColumnType("nvarchar(200)");
                entity.Property(r => r.PostalCode).IsRequired().HasColumnType("char(5)");
                entity.Property(r => r.Address).IsRequired().HasColumnType("nvarchar(200)");
                entity.Property(r => r.PhoneNumber).IsRequired().HasColumnType("varchar(30)");
                entity.Property(r => r.RegisterDate).IsRequired().HasColumnType("smalldatetime");
            });

            //Category
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(ct => ct.Name).IsRequired().HasColumnType("nvarchar(50)");
                entity.Property(ct => ct.Description).IsRequired().HasColumnType("nvarchar(200)");
                entity.Property(ct => ct.RegisterDate).IsRequired().HasColumnType("smalldatetime");
            });

            //Food
            modelBuilder.Entity<Food>(entity =>
            {
                entity.Property(f => f.Name).IsRequired().HasColumnType("nvarchar(100)");
                entity.Property(f => f.Price).IsRequired().HasColumnType("decimal(15,2)");
                entity.Property(f => f.Description).HasColumnType("nvarchar(200)");
            });

            //ApplicationUser
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasIndex(au => au.UserName).IsUnique(true);
                entity.Property(au => au.UserName).IsRequired().HasColumnType("nvarchar(100)");
                entity.Property(au => au.Name).IsRequired().HasColumnType("nvarchar(100)");
                entity.Property(au => au.SurName).IsRequired().HasColumnType("nvarchar(50)");
                entity.Property(au => au.Email).IsRequired().HasColumnType("varchar(50)");
                entity.Property(au => au.PhoneNumber).IsRequired().HasColumnType("varchar(30)");
            });


            base.OnModelCreating(modelBuilder);

            #endregion


            #region Keys and RelationShips


            //Company-State
            modelBuilder.Entity<Company>().HasKey(c => c.Id);
            modelBuilder.Entity<Company>().HasOne(c => c.State).WithMany().HasForeignKey(c => c.StateId).OnDelete(DeleteBehavior.NoAction);


            //Restaurant-State, Restaurant-Company
            modelBuilder.Entity<Restaurant>().HasKey(r => r.Id);
            modelBuilder.Entity<Restaurant>().HasOne(r => r.State).WithMany().HasForeignKey(r => r.StateId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Restaurant>().HasOne(r => r.Company).WithMany(c=> c.Restaurants).HasForeignKey(r => r.CompanyId).OnDelete(DeleteBehavior.NoAction);


            //Category-State, Category-Restaurant
            modelBuilder.Entity<Category>().HasKey(ct => ct.Id);
            modelBuilder.Entity<Category>().HasOne(ct => ct.State).WithMany().HasForeignKey(ct => ct.StateId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Category>().HasOne(ct => ct.Restaurant).WithMany(r=> r.Categories).HasForeignKey(ct => ct.RestraurantId).OnDelete(DeleteBehavior.NoAction);


            //Food-State, Food-Category
            modelBuilder.Entity<Food>().HasKey(f => f.Id);
            modelBuilder.Entity<Food>().HasOne(f => f.State).WithMany().HasForeignKey(f => f.StateId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Food>().HasOne(f => f.Category).WithMany(ct=> ct.Foods).HasForeignKey(f => f.CategoryId).OnDelete(DeleteBehavior.NoAction);


            //ApplicationUser-State, ApplicationUser-Company
            modelBuilder.Entity<ApplicationUser>().HasOne(au => au.State).WithMany().HasForeignKey(au => au.StateId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<ApplicationUser>().HasOne(au => au.Company).WithMany().HasForeignKey(au => au.CompanyId).OnDelete(DeleteBehavior.NoAction);


            //RestaurantUser-Restaurant, RestaurantUser-ApplcationUser
            modelBuilder.Entity<RestaurantUser>().HasKey(ru => new { ru.RestaurantId, ru.ApplicationUserId });
            modelBuilder.Entity<RestaurantUser>().HasOne(ru => ru.Restaurant).WithMany().OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<RestaurantUser>().HasOne(ru => ru.ApplicationUser).WithMany().OnDelete(DeleteBehavior.NoAction);

            #endregion
        }



        public DbSet<QrMenuApi.Data.Models.RestaurantUser>? RestaurantUser { get; set; }

    }
}
