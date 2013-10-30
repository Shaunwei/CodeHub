using System;
using CodeFramework.ViewControllers;
using CodeHub.ViewModels;
using MonoTouch.Dialog;

namespace CodeHub.ViewControllers
{
    public class ChangesetBranchesViewController : ViewModelCollectionDrivenViewController
    {
        public new BranchesViewModel ViewModel
        {
            get { return (BranchesViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public ChangesetBranchesViewController(string username, string slug)
        {
            Title = "Changeset Branch".t();
            SearchPlaceholder = "Search Branches".t();
            NoItemsText = "No Branches".t();
            ViewModel = new BranchesViewModel(username, slug);

            BindCollection(ViewModel, x => x.Branches, x => {
                return new StyledStringElement(x.Name, () => NavigationController.PushViewController(new ChangesetsViewController(ViewModel.Username, ViewModel.Repository, x.Name), true));
            });
        }
    }
}

