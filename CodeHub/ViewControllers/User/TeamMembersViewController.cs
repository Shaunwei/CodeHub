using MonoTouch.Dialog;
using GitHubSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using CodeHub.Controllers;
using CodeFramework.Elements;
using CodeFramework.ViewControllers;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class TeamMembersViewController : ViewModelCollectionDrivenViewController
    {
        public new TeamMembersViewModel ViewModel
        {
            get { return (TeamMembersViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

		public TeamMembersViewController(string name, ulong id) 
        {
			Title = name;
            SearchPlaceholder = "Search Members".t();
            NoItemsText = "No Members".t();
            ViewModel = new TeamMembersViewModel(id);

            BindCollection(ViewModel.Users, s => {
                StyledStringElement sse = new UserElement(s.Login, string.Empty, string.Empty, s.AvatarUrl);
                sse.Tapped += () => NavigationController.PushViewController(new ProfileViewController(s.Login), true);
                return sse;
            });
        }
    }
}