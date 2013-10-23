using MonoTouch.Dialog;
using GitHubSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using CodeHub.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;
using CodeFramework.ViewControllers;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class TeamsViewController : ViewModelCollectionDrivenViewController
    {
        public new TeamsViewModel ViewModel
        {
            get { return (TeamsViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public TeamsViewController(string name)
        {
            Title = "Teams".t();
            SearchPlaceholder = "Search Teams".t();
            NoItemsText = "No Teams".t();
            ViewModel = new TeamsViewModel(name);

            this.BindCollection(ViewModel, x => {
                return new StyledStringElement(x.Name, () => NavigationController.PushViewController(new TeamMembersViewController(x.Name, x.Id), true));
            });
        }
    }
}