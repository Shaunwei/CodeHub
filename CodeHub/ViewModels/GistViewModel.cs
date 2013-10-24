using System;
using GitHubSharp.Models;
using System.Threading.Tasks;
using CodeFramework.ViewModels;

namespace CodeHub.ViewModels
{
    public class GistViewModel : ViewModel, ILoadableViewModel
    {
        private GistModel _gist;
        private bool _starred;

        public string Id
        {
            get;
            private set;
        }

        public GistModel Gist
        {
            get { return _gist; }
            set { SetProperty(ref _gist, value); }
        }

        public bool IsStarred
        {
            get { return _starred; }
            private set { SetProperty(ref _starred, value); }
        }

        public GistViewModel(string id)
        {
            Id = id;
        }

        public async Task SetStarred(bool value)
        {
            var request = value ? Application.Client.Gists[Id].Star() : Application.Client.Gists[Id].Unstar();
            await Application.Client.ExecuteAsync(request);
            IsStarred = value;
        }

        public Task Load(bool forceDataRefresh)
        {
            var t1 = Task.Run(() => this.RequestModel(Application.Client.Gists[Id].Get(), forceDataRefresh, response => {
                Gist = response.Data;
            }));

            new Task(() => {
                try
                {
                    this.RequestModel(Application.Client.Gists[Id].IsGistStarred(), forceDataRefresh, response => {
                        IsStarred = response.Data;
                    });
                }
                catch
                {
                    // Don't care...
                }
            }).Start();

            return t1;
        }

        public async Task Edit(GistEditModel editModel)
        {
            var response = await Application.Client.ExecuteAsync(Application.Client.Gists[Id].EditGist(editModel));
            Gist = response.Data;
        }
    }
}

