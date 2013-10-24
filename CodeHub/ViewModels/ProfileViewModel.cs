using GitHubSharp.Models;
using GitHubSharp;
using CodeFramework.ViewModels;
using System.Threading.Tasks;
using CodeHub.ViewModels;

namespace CodeHub.Controllers
{
    public class ProfileViewModel : ViewModel, ILoadableViewModel
    {
        private UserModel _userModel;

        public string Username
        {
            get;
            private set;
        }

        public UserModel User
        {
            get { return _userModel; }
            private set { SetProperty(ref _userModel, value); }
        }

        public ProfileViewModel(string username)
        {
            Username = username;
        }

        public Task Load(bool forceDataRefresh)
        {
            return Task.Run(() => this.RequestModel(Application.Client.Users[Username].Get(), forceDataRefresh, response => User = response.Data));
        }
    }
}

