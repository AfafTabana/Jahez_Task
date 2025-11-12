using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Jahez_Task.Models
{
    public class AppDbContext : IdentityDbContext<ApplicationUser , IdentityRole<int>, int>
    {
        public DbSet<BookLoan> BookLoans { get; set; }

        public DbSet<OverDueNotification> OverDueNotifications { get; set; }

        public DbSet<Book> Books { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

    }
}
