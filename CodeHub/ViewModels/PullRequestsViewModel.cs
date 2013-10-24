using GitHubSharp.Models;
using CodeHub.Filters.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;
using System;

namespace CodeHub.ViewModels
{
    public class PullRequestsViewModel : ViewModel, ILoadableViewModel
    {
        private readonly FilterableCollectionViewModel<PullRequestModel, PullRequestsFilterModel> _pullrequests;
        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            protected set { SetProperty(ref _isLoading, value); }
        }

        public FilterableCollectionViewModel<PullRequestModel, PullRequestsFilterModel> PullRequests
        {
            get { return _pullrequests; }
        }

        public string Username { get; private set; }

        public string Repository { get; private set; }

        public PullRequestsViewModel(string username, string repository) 
        {
            Username = username;
            Repository = repository;

            _pullrequests = new FilterableCollectionViewModel<PullRequestModel, PullRequestsFilterModel>("PullRequests");
            _pullrequests.Bind(x => x.Filter, async () => {
                try
                {
                    IsLoading = true;
                    await Load(true);
                }
                catch (Exception e)
                {
                }
                finally
                {
                    IsLoading = false;
                }
            });
        }

        public Task Load(bool forceDataRefresh)
        {
            var state = PullRequests.Filter.IsOpen ? "open" : "closed";
            var request = Application.Client.Users[Username].Repositories[Repository].PullRequests.GetAll(state: state);
            return PullRequests.SimpleCollectionLoad(request, forceDataRefresh);
        }
    }
}
