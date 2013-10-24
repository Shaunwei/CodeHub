using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class UserFollowersViewModel : ViewModel, ILoadableViewModel
    {
        private readonly CollectionViewModel<BasicUserModel> _users = new CollectionViewModel<BasicUserModel>();

        public CollectionViewModel<BasicUserModel> Users
        {
            get { return _users; }
        }

        public string Name
        {
            get;
            private set;
        }

        public UserFollowersViewModel(string name)
        {
            Name = name;
        }

        public Task Load(bool forceDataRefresh)
        {
            return Users.SimpleCollectionLoad(Application.Client.Users[Name].GetFollowers(), forceDataRefresh);
        }
    }
}

