using System;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class UserFollowersViewController : FollowersViewController
    {
        public new UserFollowersViewModel ViewModel
        {
            get { return (UserFollowersViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public UserFollowersViewController(string username)
        {
            Title = "Followers".t();
            SearchPlaceholder = "Search Followers".t();
            NoItemsText = "No Followers".t();
            ViewModel = new UserFollowersViewModel(username);

            BindCollection(ViewModel, CreateElement);
        }
    }
}

