using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;


using System;

namespace Jahez_Task.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public DbSet<BookLoan> BookLoans { get; set; }

        public DbSet<OverDueNotification> OverDueNotifications { get; set; }

        public DbSet<Book> Books { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer().ConfigureWarnings(warnings =>
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Roles
            var AdminRoleId = 1;
            var MemberRoleId = 2;


            modelBuilder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int> { Id = AdminRoleId, Name = "admin", NormalizedName = "ADMIN" },
                new IdentityRole<int> { Id = MemberRoleId, Name = "member", NormalizedName = "MEMBER" }
            );

            // Seed Admin User
            var adminUserId = 1;
            var adminEmail = "admin@gmail.com";
            var hasher = new PasswordHasher<ApplicationUser>();



            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = adminUserId,
                    UserName = adminEmail,
                    NormalizedUserName = adminEmail.ToUpper(),
                    Email = adminEmail,
                    NormalizedEmail = adminEmail.ToUpper(),
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(null, "admin123"),
                    SecurityStamp = "a-fixed-guid-here"


                }
            );


            modelBuilder.Entity<IdentityUserRole<int>>().HasData(
               new IdentityUserRole<int>
                  {
                   UserId = adminUserId,
                   RoleId = AdminRoleId
                  }
                   );



        }
    }
}
