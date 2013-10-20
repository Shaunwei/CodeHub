using GitHubSharp.Models;
using CodeHub.Filters.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class PullRequestsViewModel : FilterableCollectionViewModel<PullRequestModel, PullRequestsFilterModel>, ILoadableViewModel
    {
        public string Username { get; private set; }

        public string Repository { get; private set; }

        public PullRequestsViewModel(string username, string repository) 
            : base("PullRequests")
        {
            Username = username;
            Repository = repository;
        }

        public async Task Load(bool forceDataRefresh)
        {
            var state = _filter.IsOpen ? "open" : "closed";
            var request = Application.Client.Users[Username].Repositories[Repository].PullRequests.GetAll(state: state);
            await this.SimpleCollectionLoad(request, forceDataRefresh);
        }

        protected override async void FilterChanged()
        {
            await Load(true);
        }
    }
}
