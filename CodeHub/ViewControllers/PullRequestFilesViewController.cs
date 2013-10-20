using CodeFramework.ViewControllers;
using CodeHub.ViewModels;
using MonoTouch.Dialog;

namespace CodeHub.ViewControllers
{
    public class PullRequestFilesViewController : ViewModelCollectionDrivenViewController
    {
        public new PullRequestFilesViewModel ViewModel
        {
            get { return (PullRequestFilesViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        public PullRequestFilesViewController(string username, string repository, ulong id)
        {
            Title = "Files";
            SearchPlaceholder = "Search Files".t();
            NoItemsText = "No Files".t();
            ViewModel = new PullRequestFilesViewModel(username, repository, id);

            BindCollection(ViewModel, x => {
                var name = x.Filename.Substring(x.Filename.LastIndexOf("/") + 1);
                var el = new StyledStringElement(name, x.Status, MonoTouch.UIKit.UITableViewCellStyle.Subtitle);
                el.Image = Images.File;
                el.Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator;
                el.Tapped += () => NavigationController.PushViewController(
                    new RawContentViewController(x.RawUrl) { Title = name }, true);
                return el;
            });
        }
    }
}



