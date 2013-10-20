using GitHubSharp.Models;
using System.Threading.Tasks;
using CodeFramework.ViewModels;
using CodeHub.ViewModels;

namespace CodeHub.Controllers
{
    public class RepositoryCollaboratorsViewModel : CollectionViewModel<BasicUserModel>, ILoadableViewModel
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

        public RepositoryCollaboratorsViewModel(string username, string repository) 
        {
            Username = username;
            Repository = repository;
        }

        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.Users[Username].Repositories[Repository].GetCollaborators(), forceDataRefresh);
        }
    }
}

