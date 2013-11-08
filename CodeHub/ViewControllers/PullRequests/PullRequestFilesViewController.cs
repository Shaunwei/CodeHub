using CodeFramework.ViewControllers;
using CodeHub.ViewModels;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

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

            BindCollection(ViewModel.Files, x => {
                var name = x.Filename.Substring(x.Filename.LastIndexOf("/") + 1);
                var el = new StyledStringElement(name, x.Status, MonoTouch.UIKit.UITableViewCellStyle.Subtitle);
                el.Image = Images.File;
                el.Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator;
                el.Tapped += () => NavigationController.PushViewController(
                    new RawContentViewController(x.ContentsUrl, null, x.Filename, x.Patch == null) { Title = name }, true);
                return el;
            });
        }

        public override Source CreateSizingSource(bool unevenRows)
        {
            return new CustomSource(this);
        }
    
        private class CustomSource : Source
        {
            public CustomSource(PullRequestFilesViewController parent)
                : base(parent)
            {
            }

            public override MonoTouch.UIKit.UIView GetViewForHeader(MonoTouch.UIKit.UITableView tableView, int sectionIdx)
            {
                var view = base.GetViewForHeader(tableView, sectionIdx);
                foreach (var v in view.Subviews)
                {
                    var label = v as UILabel;
                    if (label != null)
                    {
                        label.LineBreakMode = UILineBreakMode.HeadTruncation;
                    }
                }
                return view;
            }
        }
    }
}



