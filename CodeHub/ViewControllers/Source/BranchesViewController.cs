using MonoTouch.Dialog;
using CodeHub.ViewModels;
using CodeFramework.ViewControllers;

namespace CodeHub.ViewControllers
{
    public class BranchesViewController : ViewModelCollectionDrivenViewController
    {
        public new BranchesViewModel ViewModel
        {
            get { return (BranchesViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public BranchesViewController(string username, string slug)
        {
            Title = "Branches".t();
            SearchPlaceholder = "Search Branches".t();
            NoItemsText = "No Branches".t();
            ViewModel = new BranchesViewModel(username, slug);

            BindCollection(ViewModel, x => x.Branches, x => {
                return new StyledStringElement(x.Name, () => NavigationController.PushViewController(new SourceViewController(ViewModel.Username, ViewModel.Repository, x.Name), true));
            });
        }
    }
}
