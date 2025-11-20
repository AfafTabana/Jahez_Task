using JahezTask.Application.Interfaces.Repositories;
using JahezTask.Domain.Entities;
using JahezTask.Persistence.Data;

namespace JahezTask.Persistence.Repositories
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
