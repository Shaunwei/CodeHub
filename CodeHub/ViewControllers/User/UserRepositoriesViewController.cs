using System;
using CodeFramework.ViewControllers;
using CodeFramework.Filters.ViewControllers;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class UserRepositoriesViewController : RepositoriesViewController
    {
        public UserRepositoriesViewController(string username, bool refresh = true)
            : base(new RepositoriesViewModel(username), refresh: refresh)
        {
            ShowOwner = false;
        }
    }
}

