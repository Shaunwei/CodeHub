using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class IssueMilestonesViewModel : ViewModel, ILoadableViewModel
    {
        private CollectionViewModel<MilestoneModel> _milestones = new CollectionViewModel<MilestoneModel>();

        public CollectionViewModel<MilestoneModel> Milestones
        {
            get { return _milestones; }
        }

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

        public Task Load(bool forceDataRefresh)
        {
            return Milestones.SimpleCollectionLoad(Application.Client.Users[Username].Repositories[Repository].Milestones.GetAll(), forceDataRefresh);
        }
    }
}

