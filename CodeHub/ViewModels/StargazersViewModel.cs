using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class StargazersViewModel : CollectionViewModel<BasicUserModel>, ILoadableViewModel
    {
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

        public StargazersViewModel(string owner, string name)
        {
            User = owner;
            Repository = name;
        }

        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.Users[User].Repositories[Repository].GetStargazers(), forceDataRefresh);
        }
    }
}

