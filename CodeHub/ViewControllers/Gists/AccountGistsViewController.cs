using System;
using CodeHub.ViewModels;
using MonoTouch.UIKit;
using CodeFramework.Views;

namespace CodeHub.ViewControllers
{
    public class AccountGistsViewController : GistsViewController
    {
        public AccountGistsViewController(string username)
            : base(new AccountGistsViewModel(username))
        {
            if (username != null)
            {
                if (Application.Accounts.ActiveAccount.Username.Equals(username))
                    Title = "My Gists";
                else
                {
                    if (username.EndsWith("s"))
                        Title = username + "' Gists";
                    else
                        Title = username + "'s Gists";
                }
            }
            else
            {
                Title = "Gists";
            }

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(CodeFramework.Theme.CurrentTheme.AddButton, NewGist));
        }

        private void NewGist()
        {
            var ctrl = new CreateGistViewController();
            ctrl.Created = (newGist) =>((AccountGistsViewModel)ViewModel).Gists.Items.Insert(0, newGist);
            NavigationController.PushViewController(ctrl, true);
        }
    }
}

