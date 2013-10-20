using CodeFramework.ViewControllers;
using MonoTouch.Dialog;
using GitHubSharp.Models;
using CodeFramework.Elements;

namespace CodeHub.ViewControllers
{
    public abstract class FollowersViewController : ViewModelCollectionDrivenViewController
    {
        protected FollowersViewController()
		{
		}

        protected Element CreateElement(BasicUserModel model)
        {
            StyledStringElement sse = new UserElement(model.Login, string.Empty, string.Empty, model.AvatarUrl);
            sse.Tapped += () => NavigationController.PushViewController(new ProfileViewController(model.Login), true);
            return sse;
        }
	}
}

