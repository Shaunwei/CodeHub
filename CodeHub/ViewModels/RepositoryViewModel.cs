using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;
using CodeHub.ViewModels;
using System.Collections.Generic;

namespace CodeHub.Controllers
{
    public class RepositoryViewModel : ViewModel, ILoadableViewModel
    {
        private bool _starred;
        private bool _watched;
        private RepositoryModel _repository;
        private ContentModel _readme;
        private List<BranchModel> _branches;

        public string Username 
        { 
            get; 
            private set; 
        }

        public string RepositoryName 
        { 
            get; 
            private set; 
        }

        public bool IsStarred
        {
            get { return _starred; }
            private set { SetProperty(ref _starred, value); }
        }

        public bool IsWatched
        {
            get { return _watched; }
            private set { SetProperty(ref _watched, value); }
        }

        public RepositoryModel Repository
        {
            get { return _repository; }
            private set { SetProperty(ref _repository, value); }
        }

        public ContentModel Readme
        {
            get { return _readme; }
            private set { SetProperty(ref _readme, value); }
        }

        public List<BranchModel> Branches
        {
            get { return _branches; }
            private set { SetProperty(ref _branches, value); }
        }

        public RepositoryViewModel(string user, string repo)
        {
            Username = user;
            RepositoryName = repo;
        }


        public Task Load(bool forceDataRefresh)
        {
            var t1 = Task.Run(() => this.RequestModel(Application.Client.Users[Username].Repositories[RepositoryName].Get(), forceDataRefresh, response => {
                Repository = response.Data;
            }));

            FireAndForgetTask.Start(() => {
                this.RequestModel(Application.Client.Users[Username].Repositories[RepositoryName].GetReadme(), 
                                  forceDataRefresh, response => Readme = response.Data);
            });

            FireAndForgetTask.Start(() => {
                this.RequestModel(Application.Client.Users[Username].Repositories[RepositoryName].GetBranches(), 
                                  forceDataRefresh, response => Branches = response.Data);
            });

            FireAndForgetTask.Start(() => {
                this.RequestModel(Application.Client.Users[Username].Repositories[RepositoryName].IsWatching(), 
                                  forceDataRefresh, response => IsWatched = response.Data);
            });
         
            FireAndForgetTask.Start(() => {
                this.RequestModel(Application.Client.Users[Username].Repositories[RepositoryName].IsStarred(), 
                                  forceDataRefresh, response => IsStarred = response.Data);
            });

            return t1;
        }

        public async Task Watch()
        {
            await Application.Client.ExecuteAsync(Application.Client.Users[Username].Repositories[RepositoryName].Watch());
            IsWatched = true;
        }

        public async Task StopWatching()
        {
            await Application.Client.ExecuteAsync(Application.Client.Users[Username].Repositories[RepositoryName].StopWatching());
            IsWatched = false;
        }

        public async Task Star()
        {
            await Application.Client.ExecuteAsync(Application.Client.Users[Username].Repositories[RepositoryName].Star());
            IsStarred = true;
        }

        public async Task Unstar()
        {
            await Application.Client.ExecuteAsync(Application.Client.Users[Username].Repositories[RepositoryName].Unstar());
            IsStarred = false;
        }
    }
}

