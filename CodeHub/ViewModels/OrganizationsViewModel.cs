using GitHubSharp.Models;
using CodeFramework.ViewModels;
using CodeHub.ViewModels;
using System.Threading.Tasks;

namespace CodeHub.Controllers
{
    public class OrganizationsViewModel : CollectionViewModel<BasicUserModel>, ILoadableViewModel
	{
        public string OrganizationName 
        { 
            get; 
            private set; 
        }

        public OrganizationsViewModel(string organizationName) 
		{
            OrganizationName = organizationName;
		}

        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.Users[OrganizationName].GetOrganizations(), forceDataRefresh);
        }
	}
}

