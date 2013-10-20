using GitHubSharp.Models;
using System.Threading.Tasks;
using CodeFramework.ViewModels;
using CodeHub.ViewModels;

namespace CodeHub.Controllers
{
    public class OrganizationMembersViewModel : CollectionViewModel<BasicUserModel>, ILoadableViewModel
    {
        public string OrganizationName 
        { 
            get; 
            private set; 
        }

        public OrganizationMembersViewModel(string organizationName) 
        {
            OrganizationName = organizationName;
        }

        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.Organizations[OrganizationName].GetMembers(), forceDataRefresh);
        }
    }
}

