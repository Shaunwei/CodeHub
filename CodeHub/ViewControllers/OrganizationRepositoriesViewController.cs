using CodeHub.Controllers;
using MonoTouch.Dialog.Utilities;
using GitHubSharp.Models;
using CodeFramework.Views;
using MonoTouch.Dialog;
using CodeFramework.ViewControllers;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
	public class OrganizationRepositoriesViewController : RepositoriesViewController
	{
        public OrganizationRepositoriesViewController(string name)
            : base(new OrganizationRepositoriesViewModel(name))
        {
            ShowOwner = false;
            Title = name;
        }
	}
}

