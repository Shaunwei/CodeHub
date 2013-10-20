using System;
using GitHubSharp.Models;
using CodeHub.Controllers;
using MonoTouch.Dialog;
using CodeFramework.Elements;
using CodeFramework.ViewControllers;
using System.Linq;

namespace CodeHub.ViewControllers
{
    public class IssueAssigneesViewController : ViewModelCollectionDrivenViewController
    {
        public Action<BasicUserModel> SelectedUser;

        public new RepositoryCollaboratorsViewModel ViewModel
        {
            get { return (RepositoryCollaboratorsViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

		public IssueAssigneesViewController(string user, string repo)
        {
            Title = "Assignees".t();
            NoItemsText = "No Assignees".t();
            SearchPlaceholder = "Search Assignees".t();
            ViewModel = new RepositoryCollaboratorsViewModel(user, repo);

            //Add a fake 'Unassigned' guy so we can always unassigned what we've done
            BindCollection(ViewModel, x => x.Items, (ev) =>
            {
                var items = ViewModel.Items.ToList();
                items.Insert(0, new BasicUserModel { Id = 0, Login = "Unassigned" });

                RenderList(items, x => {
                    var e = new UserElement(x.Login, string.Empty, string.Empty, x.AvatarUrl);
                    e.Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator;
                    e.Tapped += () => {
                        if (SelectedUser != null)
                            SelectedUser(x);
                    };
                    return e;
                }, ViewModel.MoreItems);
            });
        }
    }
}

