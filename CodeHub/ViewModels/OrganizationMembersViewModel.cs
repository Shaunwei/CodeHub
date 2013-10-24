using GitHubSharp.Models;
using System.Threading.Tasks;
using CodeFramework.ViewModels;
using CodeHub.ViewModels;

namespace CodeHub.Controllers
{
    public class OrganizationMembersViewModel : ViewModel, ILoadableViewModel
    {
        private readonly CollectionViewModel<BasicUserModel> _members = new CollectionViewModel<BasicUserModel>();

        public CollectionViewModel<BasicUserModel> Members
        {
            get { return _members; }
        }

        public string OrganizationName 
        { 
            get; 
            private set; 
        }

        public OrganizationMembersViewModel(string organizationName) 
        {
            OrganizationName = organizationName;
        }

        public Task Load(bool forceDataRefresh)
        {
            return Members.SimpleCollectionLoad(Application.Client.Organizations[OrganizationName].GetMembers(), forceDataRefresh);
        }
    }
}

