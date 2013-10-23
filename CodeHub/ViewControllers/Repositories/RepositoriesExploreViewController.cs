using CodeFramework.ViewControllers;
using CodeHub.ViewModels;
using CodeFramework.Elements;
using System.Drawing;
using MonoTouch.UIKit;
using System;

namespace CodeHub.ViewControllers
{
    public sealed class RepositoriesExploreViewController : ViewModelCollectionDrivenViewController
    {
        public new RepositoriesExploreViewModel ViewModel
        {
            get { return (RepositoriesExploreViewModel)base.ViewModel; }
            private set { base.ViewModel = value; }
        }

		public RepositoriesExploreViewController()
        {
            AutoHideSearch = false;
            //EnableFilter = true;
            SearchPlaceholder = "Search Repositories".t();
            NoItemsText = "No Repositories".t();
            Title = "Explore".t();
            ViewModel = new RepositoriesExploreViewModel();

            BindCollection(ViewModel, repo => {
                var description = Application.Account.ShowRepositoryDescriptionInList ? repo.Description : string.Empty;
                var imageUrl = repo.Fork ? CodeHub.Images.GitHubRepoForkUrl : CodeHub.Images.GitHubRepoUrl;
                var sse = new RepositoryElement(repo.Name, repo.Watchers, repo.Forks, description, repo.Owner, imageUrl) { ShowOwner = true };
                sse.Tapped += () => NavigationController.PushViewController(new RepositoryViewController(repo.Owner, repo.Name, repo.Name), true);
                return sse;
            });
        }

        void ShowSearch(bool value)
        {
            if (!value)
            {
                if (TableView.ContentOffset.Y < 44)
                    TableView.ContentOffset = new PointF (0, 44);
            }
            else
            {
                TableView.ContentOffset = new PointF (0, 0);
            }
        }

        class ExploreSearchDelegate : UISearchBarDelegate 
        {
            readonly RepositoriesExploreViewController _container;

            public ExploreSearchDelegate (RepositoriesExploreViewController container)
            {
                _container = container;
            }

            public override void OnEditingStarted (UISearchBar searchBar)
            {
                searchBar.ShowsCancelButton = true;
                _container.SearchStart ();
                _container.ShowSearch(true);
                _container.NavigationController.SetNavigationBarHidden(true, true);
            }

            public override void OnEditingStopped (UISearchBar searchBar)
            {
                searchBar.ShowsCancelButton = false;
                _container.FinishSearch ();
                _container.NavigationController.SetNavigationBarHidden(false, true);
                _container.SearchEnd();
            }

            public override void TextChanged (UISearchBar searchBar, string searchText)
            {
            }

            public override void CancelButtonClicked (UISearchBar searchBar)
            {
                searchBar.ShowsCancelButton = false;
                _container.FinishSearch ();
                searchBar.ResignFirstResponder ();
                _container.NavigationController.SetNavigationBarHidden(false, true);
                _container.SearchEnd();
            }

            public override void SearchButtonClicked (UISearchBar searchBar)
            {
                _container.SearchButtonClicked (searchBar.Text);
                _container.NavigationController.SetNavigationBarHidden(false, true);
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var search = (UISearchBar)TableView.TableHeaderView;
            search.Delegate = new ExploreSearchDelegate(this);
        }

        public override void SearchButtonClicked(string text)
        {
            View.EndEditing(true);

            try
            {
                this.DoWorkTest("Searching...".t(), async () => await ViewModel.Search(text));
            }
            catch (Exception e)
            {
                MonoTouch.Utilities.ShowAlert("Error".t(), e.Message);
                MonoTouch.Utilities.LogException(e);
            }
        }
    }
}

