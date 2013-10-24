using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;
using CodeHub.ViewModels;

namespace CodeHub.Controllers
{
    public class OrganizationViewModel : ViewModel, ILoadableViewModel
	{
        private UserModel _userModel;

        public string Name 
        { 
            get; 
            private set; 
        }

		public OrganizationViewModel(string name) 
		{
			Name = name;
		}

        public UserModel Organization
        {
            get { return _userModel; }
            private set { SetProperty(ref _userModel, value); }
        }

        public Task Load(bool forceDataRefresh)
        {
            return Task.Run(() => this.RequestModel(Application.Client.Organizations[Name].Get(), forceDataRefresh, response => Organization = response.Data));
        }
	}
}

