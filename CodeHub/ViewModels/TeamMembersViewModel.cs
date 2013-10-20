using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;
using CodeHub.ViewModels;

namespace CodeHub.ViewModels
{
    public class TeamMembersViewModel : CollectionViewModel<BasicUserModel>, ILoadableViewModel
    {
        public ulong Id
        {
            get;
            private set;
        }

        public TeamMembersViewModel(ulong id)
        {
            Id = id;
        }

        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.Teams[Id].GetMembers(), forceDataRefresh);
        }
    }
}