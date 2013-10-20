using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;
using CodeHub.ViewModels;

namespace CodeHub.ViewModels
{
    public class TagsViewModel : CollectionViewModel<TagModel>, ILoadableViewModel
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

        public TagsViewModel(string user, string repository)
        {
            Username = user;
            Repository = repository;
        }

        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.Users[Username].Repositories[Repository].GetTags(), forceDataRefresh);
        }
    }
}

