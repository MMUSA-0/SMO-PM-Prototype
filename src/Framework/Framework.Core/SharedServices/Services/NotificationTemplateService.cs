using Framework.Core.Data.Repositories;
using Framework.Core.Notifications;
using Framework.Core.SharedServices.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Framework.Core.SharedServices.Services
{
    public class NotificationTemplateService
    {
        private readonly IRepositoryBase<ICommonsDbContext, NotificationTemplate> _notificationTemplateRepository;

        public NotificationTemplateService(IRepositoryBase<ICommonsDbContext, NotificationTemplate> notificationTemplateRepository)
        {
            _notificationTemplateRepository = notificationTemplateRepository;
        }

        public async Task<NotificationTemplate> GetTemplateAsync(string key, NotificationTypes notificationType)
        {
            var template =
                await _notificationTemplateRepository.TableNoTracking.FirstOrDefaultAsync(n => n.Name.Trim() == key && n.NotificationTypeId == (int)notificationType);
            if (template == null)
            {
                throw new NotificationException(
                    $"The template '{key}' is not available, Check table common.NotificationTemplate");
            }

            return template;

        }
    }
}
