using System;
using CodeHub.Controllers;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeFramework.ViewControllers;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class PullRequestsViewController : ViewModelCollectionDrivenViewController
    {
        private UISegmentedControl _viewSegment;
        private UIBarButtonItem _segmentBarButton;

        public new PullRequestsViewModel ViewModel
        {
            get { return (PullRequestsViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public PullRequestsViewController(string user, string repo)
        {
            Root.UnevenRows = true;
            Title = "Pull Requests".t();
            SearchPlaceholder = "Search Pull Requests".t();
            NoItemsText = "No Pull Requests".t();
            ViewModel = new PullRequestsViewModel(user, repo);

            BindCollection(ViewModel, s => {
                var sse = new NameTimeStringElement {
                    Name = s.Title,
                    String = s.Body.Replace('\n', ' ').Replace("\r", ""),
                    Lines = 3,
                    Time = s.CreatedAt.ToDaysAgo(),
                    Image = Theme.CurrentTheme.AnonymousUserImage,
                    ImageUri = new Uri(s.User.AvatarUrl)
                };
                sse.Tapped += () => NavigationController.PushViewController(new PullRequestViewController(ViewModel.Username, ViewModel.Repository, s.Number), true);
                return sse;
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _viewSegment = new UISegmentedControl(new string[] { "Open".t(), "Closed".t() });
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
            _viewSegment.SelectedSegment = ViewModel.Filter.IsOpen ? 0 : 1;

            _viewSegment.ValueChanged += SegmentValueChanged;
        }

        void SegmentValueChanged (object sender, EventArgs e)
        {
            if (_viewSegment.SelectedSegment == 0)
            {
                ViewModel.ApplyFilter(new CodeHub.Filters.Models.PullRequestsFilterModel { IsOpen = true }, true);
            }
            else if (_viewSegment.SelectedSegment == 1)
            {
                ViewModel.ApplyFilter(new CodeHub.Filters.Models.PullRequestsFilterModel { IsOpen = false }, true);
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

