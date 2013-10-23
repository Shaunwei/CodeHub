using CodeFramework.ViewControllers;
using CodeHub.Controllers;
using MonoTouch.Dialog;

namespace CodeHub.ViewControllers
{
    public class OrganizationsViewController : ViewModelCollectionDrivenViewController
	{
        public new OrganizationsViewModel ViewModel
        {
            get { return (OrganizationsViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

		public OrganizationsViewController(string name) 
		{
            Title = "Organizations".t();
            SearchPlaceholder = "Search Organizations".t();
            NoItemsText = "No Organizations".t();
            ViewModel = new OrganizationsViewModel(name);

            BindCollection(ViewModel, x => {
                return new StyledStringElement(x.Login, () => NavigationController.PushViewController(new OrganizationViewController(x.Login), true));
            });
		}
	}
}

