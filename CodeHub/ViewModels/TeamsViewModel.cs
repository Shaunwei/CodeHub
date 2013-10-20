using GitHubSharp.Models;
using System.Threading.Tasks;
using CodeFramework.ViewModels;
using CodeHub.ViewModels;

namespace CodeHub.ViewModels
{
    public class TeamsViewModel : CollectionViewModel<TeamShortModel>, ILoadableViewModel
    {
        public string OrganizationName
        {
            get;
            private set;
        }

        public TeamsViewModel(string name)
        {
            OrganizationName = name;
        }

        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.Organizations[OrganizationName].GetTeams(), forceDataRefresh);
        }
    }
}