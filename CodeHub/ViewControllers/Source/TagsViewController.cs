using MonoTouch.Dialog;
using CodeHub.ViewModels;
using CodeFramework.ViewControllers;

namespace CodeHub.ViewControllers
{
    public class TagsViewController : ViewModelCollectionDrivenViewController
    {
        public new TagsViewModel ViewModel
        {
            get { return (TagsViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public TagsViewController(string user, string slug)
        {
            Title = "Tags".t();
            SearchPlaceholder = "Search Tags".t();
            NoItemsText = "No Tags".t();
            EnableSearch = true;
            ViewModel = new TagsViewModel(user, slug);

            BindCollection(ViewModel.Tags, x => {
                return new StyledStringElement(x.Name, () => NavigationController.PushViewController(new SourceViewController(ViewModel.Username, ViewModel.Repository, x.Commit.Sha), true));
            });
        }
    }
}

