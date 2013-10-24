using CodeHub.Controllers;
using MonoTouch.UIKit;
using CodeFramework.Views;
using GitHubSharp.Models;
using CodeFramework.Elements;
using CodeHub.Filters.Models;
using System;
using CodeHub.Filters.ViewControllers;
using System.Threading.Tasks;
using CodeFramework.ViewControllers;

namespace CodeHub.ViewControllers
{
    public class IssuesViewController : BaseIssuesViewController
    {
        private UISegmentedControl _viewSegment;
        private UIBarButtonItem _segmentBarButton;

        public new IssuesViewModel ViewModel
        {
            get { return (IssuesViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        public IssuesViewController(string user, string slug)
        {
            ViewModel = new IssuesViewModel(user, slug);
            BindCollection(ViewModel.Issues, CreateElement);

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(Theme.CurrentTheme.AddButton, () => {
                var b = new IssueEditViewController(ViewModel.User, ViewModel.Slug) {
                    Success = (issue) => ViewModel.CreateIssue(issue)
                };
                NavigationController.PushViewController(b, true);
            }));

            ViewModel.Bind(x => x.IsLoading, Loading);
        }

        private TaskCompletionSource<bool> _loadingSource;

        /// <summary>
        /// This function is disgusting! But for some reason it works
        /// </summary>
        /// <param name="isLoading">If set to <c>true</c> is loading.</param>
        private void Loading(bool isLoading)
        {
            if (isLoading)
            {
                if (_loadingSource != null)
                    return;

                _loadingSource = new TaskCompletionSource<bool>();
                this.DoWorkTest("Loading...", () => _loadingSource.Task);
            }
            else
            {
                _loadingSource.SetResult(true);
                _loadingSource = null;
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _viewSegment = new UISegmentedControl(new string[] { "Open".t(), "Closed".t(), "Mine".t(), "Custom".t() });
            _viewSegment.ControlStyle = UISegmentedControlStyle.Bar;
            _segmentBarButton = new UIBarButtonItem(_viewSegment);
            _segmentBarButton.Width = View.Frame.Width - 10f;
            ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
        }

        protected override void SearchEnd()
        {
            base.SearchEnd();
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(false, true);
        }

        public override void ViewWillAppear(bool animated)
        {
            if (ToolbarItems != null && !IsSearching)
                NavigationController.SetToolbarHidden(false, animated);
            base.ViewWillAppear(animated);

            //Before we select which one, make sure we detach the event handler or silly things will happen
            _viewSegment.ValueChanged -= SegmentValueChanged;

            //Select which one is currently selected
            if (ViewModel.Issues.Filter.Equals(IssuesFilterModel.CreateOpenFilter()))
                _viewSegment.SelectedSegment = 0;
            else if (ViewModel.Issues.Filter.Equals(IssuesFilterModel.CreateClosedFilter()))
                _viewSegment.SelectedSegment = 1;
            else if (ViewModel.Issues.Filter.Equals(IssuesFilterModel.CreateMineFilter(Application.Account.Username)))
                _viewSegment.SelectedSegment = 2;
            else
                _viewSegment.SelectedSegment = 3;

            _viewSegment.ValueChanged += SegmentValueChanged;
        }

        void SegmentValueChanged (object sender, EventArgs e)
        {
            if (_viewSegment.SelectedSegment == 0)
            {
                ViewModel.Issues.ApplyFilter(IssuesFilterModel.CreateOpenFilter(), true);
            }
            else if (_viewSegment.SelectedSegment == 1)
            {
                ViewModel.Issues.ApplyFilter(IssuesFilterModel.CreateClosedFilter(), true);
            }
            else if (_viewSegment.SelectedSegment == 2)
            {
                ViewModel.Issues.ApplyFilter(IssuesFilterModel.CreateMineFilter(Application.Account.Username), true);
            }
            else if (_viewSegment.SelectedSegment == 3)
            {
                var filter = new IssuesFilterViewController(ViewModel.User, ViewModel.Slug, ViewModel.Issues);
                var nav = new UINavigationController(filter);
                PresentViewController(nav, true, null);
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
        }
    }
}

