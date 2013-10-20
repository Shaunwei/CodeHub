using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class IssueMilestonesViewModel : CollectionViewModel<MilestoneModel>, ILoadableViewModel
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

        public IssueMilestonesViewModel(string username, string repository)
        {
            Username = username;
            Repository = repository;
        }

        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.Users[Username].Repositories[Repository].Milestones.GetAll(), forceDataRefresh);
        }
    }
}

