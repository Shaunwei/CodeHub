using System;
using CodeHub.Controllers;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using CodeFramework.ViewControllers;

namespace CodeHub.ViewControllers
{
    public class PublicGistsViewController : GistsViewController
    {
        public PublicGistsViewController()
            : base(new PublicGistsViewModel())
        {
            Title = "Public Gists".t();
        }
    }

    public class StarredGistsViewController : GistsViewController
    {
        public StarredGistsViewController()
            : base(new StarredGistsViewModel())
        {
            Title = "Starred Gists".t();
        }
    }

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
        }
    }


    public abstract class GistsViewController : ViewModelCollectionDrivenViewController
    {
        protected GistsViewController(GistsViewModel viewModel)
        {
            SearchPlaceholder = "Search Gists".t();
            NoItemsText = "No Gists".t();
            ViewModel = viewModel;

            BindCollection(viewModel, x => {
                var str = string.IsNullOrEmpty(x.Description) ? "Gist " + x.Id : x.Description;
                var sse = new NameTimeStringElement() { 
                    Time = x.UpdatedAt.ToDaysAgo(), 
                    String = str, 
                    Lines = 4, 
                    Image = Theme.CurrentTheme.AnonymousUserImage
                };

                sse.Name = (x.User == null) ? "Anonymous" : x.User.Login;
                sse.ImageUri = (x.User == null) ? null : new Uri(x.User.AvatarUrl);
                sse.Tapped += () => NavigationController.PushViewController(new GistInfoViewController(x), true);
                return sse;
            });
        }
    }
}

