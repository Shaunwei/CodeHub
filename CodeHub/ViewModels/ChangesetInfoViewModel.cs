using System;
using GitHubSharp.Models;
using System.Linq;
using System.Collections.Generic;
using CodeFramework.ViewModels;
using CodeFramework.Utils;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class ChangesetInfoViewModel : ViewModel, ILoadableViewModel
    {
        private readonly CollectionViewModel<CommentModel> _comments = new CollectionViewModel<CommentModel>();
        private CommitModel _commitModel;

        public string Node 
        { 
            get; 
            private set;
        }

        public string User 
        { 
            get; 
            private set; 
        }

        public string Repository 
        { 
            get; 
            private set; 
        }

        public CommitModel Changeset
        {
            get { return _commitModel; }
            private set { SetProperty(ref _commitModel, value); }
        }

        public CollectionViewModel<CommentModel> Comments
        {
            get { return _comments; }
        }

        public ChangesetInfoViewModel(string user, string repository, string node)
        {
            Node = node;
            User = user;
            Repository = repository;
        }

        public Task Load(bool forceDataRefresh)
        {
            var t1 = Task.Run(() => this.RequestModel(Application.Client.Users[User].Repositories[Repository].Commits[Node].Get(), forceDataRefresh, response => Changeset = response.Data));
            FireAndForgetTask.Start(() => Comments.SimpleCollectionLoad(Application.Client.Users[User].Repositories[Repository].Commits[Node].Comments.GetAll(), forceDataRefresh));
            return t1;
        }

        public async Task AddComment(string text)
        {
            var c = await Application.Client.ExecuteAsync(Application.Client.Users[User].Repositories[Repository].Commits[Node].Comments.Create(text));
            Comments.Items.Add(c.Data);
        }
    }
}

