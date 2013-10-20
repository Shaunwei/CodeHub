using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class UserFollowingsViewModel : CollectionViewModel<BasicUserModel>, ILoadableViewModel
    {
        public string Name
        {
            get;
            private set;
        }

        public UserFollowingsViewModel(string name)
        {
            Name = name;
        }

        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.Users[Name].GetFollowing(), forceDataRefresh);
        }
    }
}

