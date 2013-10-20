using CodeFramework.ViewModels;
using GitHubSharp.Models;
using System.Threading.Tasks;
using CodeHub.ViewModels;

namespace CodeHub.ViewModels
{
    public class PullRequestCommitsViewModel : CollectionViewModel<CommitModel>, ILoadableViewModel
    {
        public string Username 
        { 
            get; 
            private set; 
        }

        public string Repository 
        { 
            get; 
            private set; 
        }

        public ulong PullRequestId 
        { 
            get; 
            private set; 
        }

        public PullRequestCommitsViewModel(string username, string repository, ulong pullRequestId)
        {
            Username = username;
            Repository = repository;
            PullRequestId = pullRequestId;
        }

        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.Users[Username].Repositories[Repository].PullRequests[PullRequestId].GetCommits(), forceDataRefresh);
        }
    }
}

