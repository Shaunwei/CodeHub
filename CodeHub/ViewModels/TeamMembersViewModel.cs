using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;
using CodeHub.ViewModels;

namespace CodeHub.ViewModels
{
    public class TeamMembersViewModel : ViewModel, ILoadableViewModel
    {
        private readonly CollectionViewModel<BasicUserModel> _users = new CollectionViewModel<BasicUserModel>();

        public CollectionViewModel<BasicUserModel> Users
        {
            get
            {
                return _users;
            }
        }

        public ulong Id
        {
            get;
            private set;
        }

        public TeamMembersViewModel(ulong id)
        {
            Id = id;
        }

        public Task Load(bool forceDataRefresh)
        {
            return Users.SimpleCollectionLoad(Application.Client.Teams[Id].GetMembers(), forceDataRefresh);
        }
    }
}