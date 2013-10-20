using GitHubSharp.Models;
using CodeHub.Filters.Models;
using System.Collections.Generic;
using System.Linq;
using CodeFramework.ViewModels;
using CodeFramework.Utils;
using System.Threading.Tasks;
using CodeHub.ViewModels;

namespace CodeHub.Controllers
{
    public class NotificationsViewModel : CollectionViewModel<NotificationModel>, ILoadableViewModel
    {
        private const bool _all = false;
        private const bool _participating = true;

        public NotificationsViewModel()
        {
            GroupingFunction = (n) => n.GroupBy(x => x.Repository.FullName);
        }

        public async Task Load(bool forceDataRefresh)
        {
            await Task.Run(() => this.RequestModel(Application.Client.Notifications.GetAll(all: _all, participating: _participating), forceDataRefresh, response => {
                Items.Reset(response.Data.Where(x => x.Unread));
                UpdateAccountNotificationsCount();
            }));
        }

        public async Task Read(NotificationModel model)
        {
            var response = await Application.Client.ExecuteAsync(Application.Client.Notifications[model.Id].MarkAsRead());
            if (response.Data) 
            {
                //We just read it
                model.Unread = false;
                Items.Remove(model);

                //Update the notifications count on the account
                UpdateAccountNotificationsCount();
            }
        }

        private void UpdateAccountNotificationsCount()
        {
            Application.Account.Notifications = Items.Sum(x => x.Unread ? 1 : 0);
        }
    }
}

