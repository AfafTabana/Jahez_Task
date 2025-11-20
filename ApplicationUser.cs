using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser<int>
{
    public virtual List<BookLoan> BookLoans { get; set; }
    public virtual List<OverDueNotification> OverDueNotifications { get; set; }
}
