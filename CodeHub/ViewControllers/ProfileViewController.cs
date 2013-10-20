using MonoTouch.Dialog;
using CodeFramework.ViewControllers;
using CodeFramework.Views;
using MonoTouch.Dialog.Utilities;
using CodeHub.Controllers;

namespace CodeHub.ViewControllers
{
    public class ProfileViewController : ViewModelDrivenViewController, IImageUpdated
    {
        private HeaderView _header;

        public new ProfileViewModel ViewModel
        {
            get { return (ProfileViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        public ProfileViewController(string username)
        {
            Title = username;
            ViewModel = new ProfileViewModel(username);

            Bind(ViewModel, x => x.User, () => {
                _header.Subtitle = string.IsNullOrEmpty(ViewModel.User.Name) ? ViewModel.User.Login : ViewModel.User.Name;
                _header.Image = ImageLoader.DefaultRequestImage(new System.Uri(ViewModel.User.AvatarUrl), this);
                _header.SetNeedsDisplay();
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var username = ViewModel.Username;
            _header = new HeaderView(View.Bounds.Width) { Title = username };
            Root.Add(new Section(_header));

            var followers = new StyledStringElement("Followers".t(), () => NavigationController.PushViewController(new UserFollowersViewController(username), true), Images.Heart);
            var following = new StyledStringElement("Following".t(), () => NavigationController.PushViewController(new UserFollowingsViewController(username), true), Images.Following);
            var events = new StyledStringElement("Events".t(), () => NavigationController.PushViewController(new UserEventsViewController(username), true), Images.Event);
            var organizations = new StyledStringElement("Organizations".t(), () => NavigationController.PushViewController(new OrganizationsViewController(username), true), Images.Group);
            var repos = new StyledStringElement("Repositories".t(), () => NavigationController.PushViewController(new UserRepositoriesViewController(username), true), Images.Repo);
            var gists = new StyledStringElement("Gists", () => NavigationController.PushViewController(new AccountGistsViewController(username), true), Images.Script);

            Root.Add(new [] { new Section { events, organizations, followers, following }, new Section { repos, gists } });
        }

        public void UpdatedImage (System.Uri uri)
        {
            _header.Image = ImageLoader.DefaultRequestImage(uri, this);
            if (_header.Image != null)
                _header.SetNeedsDisplay();
        }
    }
}

