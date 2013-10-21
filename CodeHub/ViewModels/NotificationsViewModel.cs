using GitHubSharp.Models;
using CodeHub.Filters.Models;
using System.Collections.Generic;
using System.Linq;
using CodeFramework.ViewModels;
using CodeFramework.Utils;
using System.Threading.Tasks;
using CodeHub.ViewModels;
using System;

namespace CodeHub.Controllers
{
    public class NotificationsViewModel : FilterableCollectionViewModel<NotificationModel, NotificationsFilterModel>, ILoadableViewModel
    {
        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            protected set { SetProperty(ref _loading, value); }
        }

        public NotificationsViewModel()
            : base("Notifications")
        {
            GroupingFunction = (n) => n.GroupBy(x => x.Repository.FullName);
        }

        protected override async void FilterChanged()
        {
            Loading = true;
            try
            {
                await Load(true);
            }
            catch (Exception e)
            {
            }
            finally
            {
                Loading = false;
            }
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

                // Only remove if we're not looking at all
                if (Filter.All == false)
                    Items.Remove(model);

                //Update the notifications count on the account
                UpdateAccountNotificationsCount();
            }
        }

        private void UpdateAccountNotificationsCount()
        {
            // Only update if we're looking at 
            if (Filter.All == false && Filter.Participating == false)
                Application.Account.Notifications = Items.Sum(x => x.Unread ? 1 : 0);
        }
    }
}

