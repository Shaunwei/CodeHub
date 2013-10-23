using System;
using GitHubSharp.Models;
using System.Collections.Generic;
using CodeHub.ViewModels;
using CodeFramework.Filters.ViewControllers;

namespace CodeHub.ViewControllers
{
	public class RepositoriesStarredViewController : RepositoriesViewController
    {
		public RepositoriesStarredViewController()
            : base(new RepositoriesStarredViewModel())
        {
            ShowOwner = true;
            Title = "Following".t();
        }
    }
}

