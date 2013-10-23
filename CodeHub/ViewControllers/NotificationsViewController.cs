using System;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using CodeHub.Controllers;
using CodeFramework.ViewControllers;
using MonoTouch.UIKit;
using CodeHub.Filters.Models;
using System.Threading.Tasks;

namespace CodeHub.ViewControllers
{
	public class NotificationsViewController : ViewModelCollectionDrivenViewController
    {
        private readonly UISegmentedControl _viewSegment;
        private readonly UIBarButtonItem _segmentBarButton;

        public new NotificationsViewModel ViewModel
        {
            get { return (NotificationsViewModel)base.ViewModel; }
            private set { base.ViewModel = value; }
        }

		public NotificationsViewController()
        {
            SearchPlaceholder = "Search Notifications".t();
            NoItemsText = "No Notifications".t();
            Title = "Notifications".t();
            ViewModel = new NotificationsViewModel();

            _viewSegment = new UISegmentedControl(new string[] { "Unread".t(), "Participating".t(), "All".t() });
            _viewSegment.ControlStyle = UISegmentedControlStyle.Bar;
            _segmentBarButton = new UIBarButtonItem(_viewSegment);

            BindCollection(ViewModel, CreateElement);
            ViewModel.Bind(x => x.Loading, Loading);
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

            _segmentBarButton.Width = View.Frame.Width - 10f;
            ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
        }

		private Element CreateElement(NotificationModel x)
        {
            var el = new StyledStringElement(x.Subject.Title, x.UpdatedAt.ToDaysAgo(), MonoTouch.UIKit.UITableViewCellStyle.Subtitle);
            el.Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator;

            var subject = x.Subject.Type.ToLower();
            if (subject.Equals("issue"))
            {
                el.Image = Images.Flag;
                el.Tapped += () => {
                    this.DoWorkNoHud(() => ViewModel.Read(x));
                    var node = x.Subject.Url.Substring(x.Subject.Url.LastIndexOf('/') + 1);
                    NavigationController.PushViewController(new IssueViewController(x.Repository.Owner.Login, x.Repository.Name, ulong.Parse(node)), true);
                };
            }
            else if (subject.Equals("pullrequest"))
            {
                el.Image = Images.Hand;
                el.Tapped += () => {
                    this.DoWorkNoHud(() => ViewModel.Read(x));
                    var node = x.Subject.Url.Substring(x.Subject.Url.LastIndexOf('/') + 1);
                    NavigationController.PushViewController(new PullRequestViewController(x.Repository.Owner.Login, x.Repository.Name, ulong.Parse(node)), true);
                };
            }
            else if (subject.Equals("commit"))
            {
                el.Image = Images.Commit;
                el.Tapped += () => {
                    this.DoWorkNoHud(() => ViewModel.Read(x));
                    var node = x.Subject.Url.Substring(x.Subject.Url.LastIndexOf('/') + 1);
                    NavigationController.PushViewController(new ChangesetViewController(x.Repository.Owner.Login, x.Repository.Name, node), true);
                };
            }

            return el;
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
            if (ViewModel.Filter.Equals(NotificationsFilterModel.CreateUnreadFilter()))
                _viewSegment.SelectedSegment = 0;
            else if (ViewModel.Filter.Equals(NotificationsFilterModel.CreateParticipatingFilter()))
                _viewSegment.SelectedSegment = 1;
            else
                _viewSegment.SelectedSegment = 2;

            _viewSegment.ValueChanged += SegmentValueChanged;
        }

        void SegmentValueChanged (object sender, EventArgs e)
        {
            if (_viewSegment.SelectedSegment == 0)
            {
                ViewModel.ApplyFilter(NotificationsFilterModel.CreateUnreadFilter(), true);
            }
            else if (_viewSegment.SelectedSegment == 1)
            {
                ViewModel.ApplyFilter(NotificationsFilterModel.CreateParticipatingFilter(), true);
            }
            else if (_viewSegment.SelectedSegment == 2)
            {
                ViewModel.ApplyFilter(NotificationsFilterModel.CreateAllFilter(), true);
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
            base.ViewWillDisappear(animated);
        }

    }
}

