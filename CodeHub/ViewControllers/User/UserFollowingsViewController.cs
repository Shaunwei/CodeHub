using System;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using CodeFramework.Elements;
using CodeHub.ViewModels;
using CodeFramework.ViewControllers;

namespace CodeHub.ViewControllers
{
    public class UserFollowingsViewController : ViewModelCollectionDrivenViewController
    {
        public new UserFollowingsViewModel ViewModel
        {
            get { return (UserFollowingsViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public UserFollowingsViewController(string user)
        {
            Title = "Following".t();
            SearchPlaceholder = "Search Following".t();
            NoItemsText = "Not Following Anyone".t();
            ViewModel = new UserFollowingsViewModel(user);

            this.BindCollection(ViewModel.Users, s => {
                StyledStringElement sse = new UserElement(s.Login, string.Empty, string.Empty, s.AvatarUrl);
                sse.Tapped += () => NavigationController.PushViewController(new ProfileViewController(s.Login), true);
                return sse;
            });
        }
    }
}

