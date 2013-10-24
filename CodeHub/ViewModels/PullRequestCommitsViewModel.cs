using CodeFramework.ViewModels;
using GitHubSharp.Models;
using System.Threading.Tasks;
using CodeHub.ViewModels;

namespace CodeHub.ViewModels
{
    public class PullRequestCommitsViewModel : ChangesetViewModel
    {
        public ulong PullRequestId 
        { 
            get; 
            private set; 
        }

        public PullRequestCommitsViewModel(string username, string repository, ulong pullRequestId)
            : base(username, repository)
        {
            PullRequestId = pullRequestId;
        }

        protected override GitHubSharp.GitHubRequest<System.Collections.Generic.List<CommitModel>> GetRequest()
        {
            return Application.Client.Users[Username].Repositories[Repository].PullRequests[PullRequestId].GetCommits();
        }
    }
}

