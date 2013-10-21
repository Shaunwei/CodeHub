using CodeFramework.ViewControllers;
using CodeFramework.ViewModels;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using CodeFramework.Elements;
using CodeFramework.Filters.ViewControllers;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public abstract class RepositoriesViewController : ViewModelCollectionDrivenViewController
    {
        public bool ShowOwner { get; set; }

        public new RepositoriesViewModel ViewModel
        {
            get { return (RepositoriesViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        public RepositoriesViewController(RepositoriesViewModel viewModel, bool refresh = true)
            : base(refresh: refresh)
        {
            ShowOwner = false;
            EnableFilter = true;
            Title = "Repositories".t();
            SearchPlaceholder = "Search Repositories".t();
            NoItemsText = "No Repositories".t();
            ViewModel = viewModel;

            BindCollection(ViewModel, CreateElement);
        }

        protected Element CreateElement(RepositoryModel repo)
        {
            var description = Application.Account.ShowRepositoryDescriptionInList ? repo.Description : string.Empty;
            var imageUrl = repo.Fork ? CodeHub.Images.GitHubRepoForkUrl : CodeHub.Images.GitHubRepoUrl;
            var sse = new RepositoryElement(repo.Name, repo.Watchers, repo.Forks, description, repo.Owner.Login, imageUrl) { ShowOwner = ShowOwner };
            sse.Tapped += () => NavigationController.PushViewController(new RepositoryViewController(repo.Owner.Login, repo.Name), true);
            return sse;
        }

        protected override FilterViewController CreateFilterController()
        {
            return new CodeHub.Filters.ViewControllers.RepositoriesFilterViewController(ViewModel);
        }
    }
}