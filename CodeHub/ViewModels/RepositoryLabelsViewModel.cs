using GitHubSharp.Models;
using System.Threading.Tasks;
using CodeFramework.ViewModels;

namespace CodeHub.ViewModels
{
    public class RepositoryLabelsViewModel : CollectionViewModel<LabelModel>, ILoadableViewModel
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

        public RepositoryLabelsViewModel(string user, string repository)
        {
            Username = user;
            Repository = repository;
        }

        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.Users[Username].Repositories[Repository].GetLabels(), forceDataRefresh);
        }
    }
}

