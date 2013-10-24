using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class UserFollowingsViewModel : ViewModel, ILoadableViewModel
    {
        private readonly CollectionViewModel<BasicUserModel> _users = new CollectionViewModel<BasicUserModel>();

        public string Name
        {
            get;
            private set;
        }

        public CollectionViewModel<BasicUserModel> Users
        {
            get { return _users; }
        }

        public UserFollowingsViewModel(string name)
        {
            Name = name;
        }

        public Task Load(bool forceDataRefresh)
        {
            return _users.SimpleCollectionLoad(Application.Client.Users[Name].GetFollowing(), forceDataRefresh);
        }
    }
}

