using GitHubSharp.Models;
using System.Threading.Tasks;
using CodeFramework.ViewModels;

namespace CodeHub.ViewModels
{
    public class RepositoryLabelsViewModel : ViewModel, ILoadableViewModel
    {
        private readonly CollectionViewModel<LabelModel> _labels = new CollectionViewModel<LabelModel>();

        public CollectionViewModel<LabelModel> Labels
        {
            get { return _labels; }
        }

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

        public Task Load(bool forceDataRefresh)
        {
            return Labels.SimpleCollectionLoad(Application.Client.Users[Username].Repositories[Repository].GetLabels(), forceDataRefresh);
        }
    }
}

