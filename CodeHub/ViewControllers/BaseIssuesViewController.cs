using System;
using GitHubSharp.Models;
using CodeFramework.Elements;
using CodeFramework.ViewControllers;
using CodeFramework.ViewModels;

namespace CodeHub.ViewControllers
{
    public abstract class BaseIssuesViewController : ViewModelCollectionDrivenViewController
    {
        public new CollectionViewModel<IssueModel> ViewModel
        {
            get { return (CollectionViewModel<IssueModel>)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        protected BaseIssuesViewController(CollectionViewModel<IssueModel> viewModel)
        {
            Root.UnevenRows = true;
            Title = "Issues".t();
            SearchPlaceholder = "Search Issues".t();
            ViewModel = viewModel;
            BindCollection(viewModel, CreateElement);
        }

        public MonoTouch.Dialog.Element CreateElement(IssueModel x)
        {
            var isPullRequest = x.PullRequest != null && !(string.IsNullOrEmpty(x.PullRequest.HtmlUrl));
            var assigned = x.Assignee != null ? x.Assignee.Login : "unassigned";
            var kind = isPullRequest ? "Pull" : "Issue";
            var commentString = x.Comments == 1 ? "1 comment".t() : x.Comments + " comments".t();
            var el = new IssueElement(x.Number.ToString(), x.Title, assigned, x.State, commentString, kind, x.UpdatedAt);
            el.Tag = x;

            if (isPullRequest)
            {
                el.Tapped += () => {
                    //Make sure the first responder is gone.
                    View.EndEditing(true);
                    var s1 = x.Url.Substring(x.Url.IndexOf("/repos/") + 7);
                    var repoId = new CodeHub.Utils.RepositoryIdentifier(s1.Substring(0, s1.IndexOf("/issues")));
                    var info = new PullRequestViewController(repoId.Owner, repoId.Name, x.Number);
                    //info.Controller.ModelChanged = newModel => ChildChangedModel(newModel, x);
                    NavigationController.PushViewController(info, true);
                };
            }
            else
            {
                el.Tapped += () => {
                    //Make sure the first responder is gone.
                    View.EndEditing(true);
                    var s1 = x.Url.Substring(x.Url.IndexOf("/repos/") + 7);
                    var repoId = new CodeHub.Utils.RepositoryIdentifier(s1.Substring(0, s1.IndexOf("/issues")));
                    var info = new IssueViewController(repoId.Owner, repoId.Name, x.Number);
                    info.ViewModel.ModelChanged = newModel => ChildChangedModel(newModel, x);
                    NavigationController.PushViewController(info, true);
                };
            }
            return el;
        }

        protected abstract void ChildChangedModel(IssueModel changedModel, IssueModel oldModel);
    }
}

