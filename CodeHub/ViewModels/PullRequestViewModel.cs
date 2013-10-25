using System.Collections.Generic;
using GitHubSharp.Models;
using CodeFramework.ViewModels;
using CodeFramework.Utils;
using System.Threading.Tasks;
using System;

namespace CodeHub.ViewModels
{
    public class PullRequestViewModel : ViewModel, ILoadableViewModel
    {
        private PullRequestModel _model;
        private CollectionViewModel<IssueCommentModel> _comments = new CollectionViewModel<IssueCommentModel>();

        public string User 
        { 
            get; 
            private set; 
        }

        public string Repo 
        { 
            get; 
            private set; 
        }

        public ulong PullRequestId 
        { 
            get; 
            private set; 
        }

        public PullRequestModel PullRequest 
        { 
            get { return _model; }
            set { SetProperty(ref _model, value); }
        }

        public CollectionViewModel<IssueCommentModel> Comments
        {
            get { return _comments; }
        }

        public PullRequestViewModel(string username, string repository, ulong pullRequestId)
        {
            User = username;
            Repo = repository;
            PullRequestId = pullRequestId;
        }

        public Task Load(bool forceDataRefresh)
        {
            var pullRequest = Application.Client.Users[User].Repositories[Repo].PullRequests[PullRequestId].Get();
            var commentsRequest = Application.Client.Users[User].Repositories[Repo].Issues[PullRequestId].GetComments();

            var t1 = Task.Run(() => this.RequestModel(pullRequest, forceDataRefresh, response => PullRequest = response.Data));

            FireAndForgetTask.Start(() => this.RequestModel(commentsRequest, forceDataRefresh, response => {
                Comments.Items.Reset(response.Data);
                this.CreateMore(response, m => Comments.MoreItems = m, d => Comments.Items.AddRange(d));
            }));

            return t1;
        }

        public async Task AddComment(string text)
        {
            var comment = await Application.Client.ExecuteAsync(Application.Client.Users[User].Repositories[Repo].Issues[PullRequestId].CreateComment(text));
            Comments.Items.Add(comment.Data);
        }

        public async Task Merge()
        {
            var response = await Application.Client.ExecuteAsync(Application.Client.Users[User].Repositories[Repo].PullRequests[PullRequestId].Merge());
            if (!response.Data.Merged)
                throw new Exception(response.Data.Message);

            var pullRequest = Application.Client.Users[User].Repositories[Repo].PullRequests[PullRequestId].Get();
            await Task.Run(() => this.RequestModel(pullRequest, true, r => PullRequest = r.Data));
        }
    }
}
