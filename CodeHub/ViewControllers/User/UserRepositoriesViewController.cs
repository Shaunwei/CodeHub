using System;
using CodeFramework.ViewControllers;
using CodeFramework.Filters.ViewControllers;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class UserRepositoriesViewController : RepositoriesViewController
    {
        public UserRepositoriesViewController(string username)
            : base(new RepositoriesViewModel(username))
        {
            ShowOwner = false;
        }
    }
}

