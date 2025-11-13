using Jahez_Task.Models;
using Jahez_Task.Repository.GenericRepo;

namespace Jahez_Task.Repository.NotificationRepo
{
    public class NotificationRepository : GenericRepository<OverDueNotification> , INotificationRepository
    {
        private readonly AppDbContext appDbContext;
        public NotificationRepository(AppDbContext context) : base(context)
        {
            appDbContext = context;
        }
    }
    
}
