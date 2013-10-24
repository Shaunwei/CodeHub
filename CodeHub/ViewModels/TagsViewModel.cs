using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;
using CodeHub.ViewModels;

namespace CodeHub.ViewModels
{
    public class TagsViewModel : ViewModel, ILoadableViewModel
    {
        public CollectionViewModel<TagModel> _tags = new CollectionViewModel<TagModel>();

        public CollectionViewModel<TagModel> Tags
        {
            get { return _tags; }
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

        public TagsViewModel(string user, string repository)
        {
            Username = user;
            Repository = repository;
        }

        public Task Load(bool forceDataRefresh)
        {
            return Tags.SimpleCollectionLoad(Application.Client.Users[Username].Repositories[Repository].GetTags(), forceDataRefresh);
        }
    }
}

