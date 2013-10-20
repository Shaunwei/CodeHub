using System;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using CodeHub.Controllers;
using CodeFramework.ViewControllers;

namespace CodeHub.ViewControllers
{
	public class NotificationsViewController : ViewModelCollectionDrivenViewController
    {
        public new NotificationsViewModel ViewModel
        {
            get { return (NotificationsViewModel)base.ViewModel; }
            private set { base.ViewModel = value; }
        }

		public NotificationsViewController()
        {
            SearchPlaceholder = "Search Notifications".t();
            NoItemsText = "No Notifications".t();
            Title = "Notifications".t();
            ViewModel = new NotificationsViewModel();

            BindCollection(ViewModel, CreateElement);
        }

		public Element CreateElement(NotificationModel x)
        {
            var el = new StyledStringElement(x.Subject.Title, x.UpdatedAt.ToDaysAgo(), MonoTouch.UIKit.UITableViewCellStyle.Subtitle);
            el.Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator;

            var subject = x.Subject.Type.ToLower();
            if (subject.Equals("issue"))
            {
                el.Image = Images.Flag;
                el.Tapped += () => {
                    this.DoWorkNoHud(() => ViewModel.Read(x));
                    var node = x.Subject.Url.Substring(x.Subject.Url.LastIndexOf('/') + 1);
                    NavigationController.PushViewController(new IssueViewController(x.Repository.Owner.Login, x.Repository.Name, ulong.Parse(node)), true);
                };
            }
            else if (subject.Equals("pullrequest"))
            {
                el.Image = Images.Hand;
                el.Tapped += () => {
                    this.DoWorkNoHud(() => ViewModel.Read(x));
                    var node = x.Subject.Url.Substring(x.Subject.Url.LastIndexOf('/') + 1);
                    NavigationController.PushViewController(new PullRequestViewController(x.Repository.Owner.Login, x.Repository.Name, ulong.Parse(node)), true);
                };
            }
            else if (subject.Equals("commit"))
            {
                el.Image = Images.Commit;
                el.Tapped += () => {
                    this.DoWorkNoHud(() => ViewModel.Read(x));
                    var node = x.Subject.Url.Substring(x.Subject.Url.LastIndexOf('/') + 1);
                    NavigationController.PushViewController(new ChangesetInfoViewController(x.Repository.Owner.Login, x.Repository.Name, node), true);
                };
            }

            return el;
        }
    }
}

