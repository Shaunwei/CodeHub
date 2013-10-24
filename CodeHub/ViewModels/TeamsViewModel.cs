using GitHubSharp.Models;
using System.Threading.Tasks;
using CodeFramework.ViewModels;
using CodeHub.ViewModels;

namespace CodeHub.ViewModels
{
    public class TeamsViewModel : ViewModel, ILoadableViewModel
    {
        private readonly CollectionViewModel<TeamShortModel> _teams = new CollectionViewModel<TeamShortModel>();

        public CollectionViewModel<TeamShortModel> Teams
        {
            get { return _teams; }
        }

        public string OrganizationName
        {
            get;
            private set;
        }

        public TeamsViewModel(string name)
        {
            OrganizationName = name;
        }

        public Task Load(bool forceDataRefresh)
        {
            return Teams.SimpleCollectionLoad(Application.Client.Organizations[OrganizationName].GetTeams(), forceDataRefresh);
        }
    }
}