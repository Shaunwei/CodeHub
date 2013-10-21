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
    public class NotificationsViewModel : FilterableCollectionViewModel<NotificationModel, NotificationsFilterModel>, ILoadableViewModel
    {
        public NotificationsViewModel()
            : base("Notifications")
        {
            GroupingFunction = (n) => n.GroupBy(x => x.Repository.FullName);
        }

        protected override void FilterChanged()
        {
            Load(true);
        }

        public async Task Load(bool forceDataRefresh)
        {
            await Task.Run(() => this.RequestModel(Application.Client.Notifications.GetAll(all: Filter.All, participating: Filter.Participating), forceDataRefresh, response => {
                Items.Reset(response.Data);
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

