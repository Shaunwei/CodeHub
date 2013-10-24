using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class StargazersViewModel : ViewModel, ILoadableViewModel
    {
        private readonly CollectionViewModel<BasicUserModel> _stargazers = new CollectionViewModel<BasicUserModel>();

        public CollectionViewModel<BasicUserModel> Stargazers
        {
            get
            {
                return _stargazers;
            }
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

        public StargazersViewModel(string owner, string name)
        {
            User = owner;
            Repository = name;
        }

        public Task Load(bool forceDataRefresh)
        {
            return Stargazers.SimpleCollectionLoad(Application.Client.Users[User].Repositories[Repository].GetStargazers(), forceDataRefresh);
        }
    }
}

