using System;
using GitHubSharp.Models;
using System.Collections.Generic;
using CodeHub.Filters.Models;
using System.Linq;
using CodeFramework.ViewModels;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class PullRequestFilesViewModel : CollectionViewModel<CommitModel.CommitFileModel>, ILoadableViewModel
    {
        public ulong PullRequestId { get; private set; }
        public string Username { get; private set; }
        public string Repository { get; private set; }

        public PullRequestFilesViewModel(string username, string repository, ulong pullRequestId)
        {
            Username = username;
            Repository = repository;
            PullRequestId = pullRequestId;
        }

        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.Users[Username].Repositories[Repository].PullRequests[PullRequestId].GetFiles(), forceDataRefresh);
        }
    }
}

