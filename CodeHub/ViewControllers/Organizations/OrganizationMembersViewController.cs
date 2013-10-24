using CodeFramework.ViewControllers;
using CodeHub.Controllers;
using CodeFramework.Elements;
using MonoTouch.Dialog;

namespace CodeHub.ViewControllers
{
    public class OrganizationMembersViewController : ViewModelCollectionDrivenViewController
    {
        public new OrganizationMembersViewModel ViewModel
        {
            get { return (OrganizationMembersViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public OrganizationMembersViewController(string name)
        {
            SearchPlaceholder = "Search Memebers".t();
            NoItemsText = "No Members".t();
            Title = name;
            ViewModel = new OrganizationMembersViewModel(name);

            BindCollection(ViewModel.Members, s => {
                StyledStringElement sse = new UserElement(s.Login, string.Empty, string.Empty, s.AvatarUrl);
                sse.Tapped += () => NavigationController.PushViewController(new ProfileViewController(s.Login), true);
                return sse;
            });
        }
    }
}

