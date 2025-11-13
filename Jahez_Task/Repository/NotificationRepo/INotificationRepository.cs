using Jahez_Task.Models;
using Jahez_Task.Repository.GenericRepo;

namespace Jahez_Task.Repository.NotificationRepo
{
    public interface INotificationRepository : IGenericRepository<OverDueNotification>
    {
    }
}
