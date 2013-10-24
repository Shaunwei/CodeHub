using GitHubSharp.Models;
using System.Threading.Tasks;
using CodeFramework.ViewModels;
using CodeHub.ViewModels;

namespace CodeHub.Controllers
{
    public class RepositoryCollaboratorsViewModel : ViewModel, ILoadableViewModel
    {
        private readonly CollectionViewModel<BasicUserModel> _collaborators = new CollectionViewModel<BasicUserModel>();

        public CollectionViewModel<BasicUserModel> Collaborators
        {
            get { return _collaborators; }
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

        public RepositoryCollaboratorsViewModel(string username, string repository) 
        {
            Username = username;
            Repository = repository;
        }

        public Task Load(bool forceDataRefresh)
        {
            return Collaborators.SimpleCollectionLoad(Application.Client.Users[Username].Repositories[Repository].GetCollaborators(), forceDataRefresh);
        }
    }
}

