using GitHubSharp.Models;
using CodeFramework.ViewModels;
using CodeHub.ViewModels;
using System.Threading.Tasks;

namespace CodeHub.Controllers
{
    public class OrganizationsViewModel : ViewModel, ILoadableViewModel
	{
        private readonly CollectionViewModel<BasicUserModel> _orgs = new CollectionViewModel<BasicUserModel>();

        public CollectionViewModel<BasicUserModel> Organizations
        {
            get { return _orgs; }
        }

        public string OrganizationName 
        { 
            get; 
            private set; 
        }

        public OrganizationsViewModel(string organizationName) 
		{
            OrganizationName = organizationName;
		}

        public Task Load(bool forceDataRefresh)
        {
            return Organizations.SimpleCollectionLoad(Application.Client.Users[OrganizationName].GetOrganizations(), forceDataRefresh);
        }
	}
}

