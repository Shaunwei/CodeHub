using MonoTouch.Dialog;
using GitHubSharp.Models;
using CodeHub.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CodeHub.Filters.Models;
using System;
using CodeFramework.ViewControllers;

namespace CodeHub.ViewControllers
{
    public class SourceViewController : ViewModelCollectionDrivenViewController
    {
        public new SourceViewModel ViewModel
        {
            get { return (SourceViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        public SourceViewController(string username, string slug, string branch = "master", string path = "")
        {
            EnableSearch = true;
            EnableFilter = true;
            SearchPlaceholder = "Search Files & Folders".t();
            Title = string.IsNullOrEmpty(path) ? "Source".t() : path.Substring(path.LastIndexOf('/') + 1);
            ViewModel = new SourceViewModel(username, slug, branch, path);
            BindCollection(ViewModel.Content, CreateElement);
        }

        private Element CreateElement(ContentModel x)
        {
            if (x.Type.Equals("dir", StringComparison.OrdinalIgnoreCase))
            {
                return new StyledStringElement(x.Name, () => NavigationController.PushViewController(
                    new SourceViewController(ViewModel.Username, ViewModel.Repository, ViewModel.Branch, x.Path), true), Images.Folder);
            }
            else if (x.Type.Equals("file", StringComparison.OrdinalIgnoreCase))
            {
                //If there's a size, it's a file
                if (x.Size != null)
                {
                    return new StyledStringElement(x.Name, () => NavigationController.PushViewController(
                        new SourceInfoViewController(x.HtmlUrl, x.Path) { Title = x.Name }, true), Images.File);
                }
                //If there is no size, it's most likey a submodule
                else
                {
                    var nameAndSlug = x.GitUrl.Substring(x.GitUrl.IndexOf("/repos/") + 7);
                    var repoId = new CodeHub.Utils.RepositoryIdentifier(nameAndSlug.Substring(0, nameAndSlug.IndexOf("/git")));
                    var sha = x.GitUrl.Substring(x.GitUrl.LastIndexOf("/") + 1);
                    return new StyledStringElement(x.Name, () => NavigationController.PushViewController(
                        new SourceViewController(repoId.Owner, repoId.Name, sha) { Title = x.Name }, true), Images.Repo);
                }
            }
            else
            {
                return new StyledStringElement(x.Name) { Image = Images.File };
            }
        }

        protected override CodeFramework.Filters.ViewControllers.FilterViewController CreateFilterController()
        {
            return new CodeHub.Filters.ViewControllers.SourceFilterViewController(ViewModel.Content);
        }
    }
}

