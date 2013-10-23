using System;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class StargazersViewController : FollowersViewController
    {
        public new StargazersViewModel ViewModel
        {
            get { return (StargazersViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public StargazersViewController(string username, string slug)
        {
            Title = "Stargazers".t();
            SearchPlaceholder = "Search Stargazers".t();
            NoItemsText = "No Stargazers".t();
            ViewModel = new StargazersViewModel(username, slug);

            BindCollection(ViewModel, CreateElement);
        }
    }
}

