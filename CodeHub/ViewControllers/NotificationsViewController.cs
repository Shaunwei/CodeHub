using System;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using CodeHub.Controllers;
using CodeFramework.ViewControllers;
using MonoTouch.UIKit;
using CodeHub.Filters.Models;
using System.Threading.Tasks;
using CodeFramework.Views;
using System.Drawing;

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

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(Theme.CurrentTheme.CheckButton, MarkAllAsRead));
            NavigationItem.RightBarButtonItem.Enabled = false;

            BindCollection(ViewModel.Notifications, CreateElement);
            ViewModel.Bind(x => x.IsLoading, Loading);


            ViewModel.Notifications.CollectionChanged += (sender, e) => {
                InvokeOnMainThread(() => {
                    NavigationItem.RightBarButtonItem.Enabled = (_viewSegment.SelectedSegment == 0 || _viewSegment.SelectedSegment == 1) && ViewModel.Notifications.Items.Count > 0;
                });
            };
        }

        private async void MarkAllAsRead()
        {
            try
            {
                await ViewModel.MarkAllAsRead();
            }
            catch (Exception ex)
            {
                MonoTouch.Utilities.ShowAlert("Error".t(), ex.Message);
                MonoTouch.Utilities.LogException(ex);
            }
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

        protected override UIView CreateTableSectionView(string key)
        {
            Action a = null;

            if (_viewSegment.SelectedSegment != 2)
            {
                a = () => ClearForRepo(key);
            }
            return new NotificationSectionView(key, a);
        }

        private async void ClearForRepo(string s)
        {
            try
            {
                await ViewModel.MarkAllAsRead(s);
            }
            catch (Exception ex)
            {
                MonoTouch.Utilities.ShowAlert("Error".t(), ex.Message);
                MonoTouch.Utilities.LogException(ex);
            }
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
            if (ViewModel.Notifications.Filter.Equals(NotificationsFilterModel.CreateUnreadFilter()))
                _viewSegment.SelectedSegment = 0;
            else if (ViewModel.Notifications.Filter.Equals(NotificationsFilterModel.CreateParticipatingFilter()))
                _viewSegment.SelectedSegment = 1;
            else
                _viewSegment.SelectedSegment = 2;

            _viewSegment.ValueChanged += SegmentValueChanged;
        }

        void SegmentValueChanged (object sender, EventArgs e)
        {
            if (_viewSegment.SelectedSegment == 0)
            {
                ViewModel.Notifications.ApplyFilter(NotificationsFilterModel.CreateUnreadFilter(), true);
            }
            else if (_viewSegment.SelectedSegment == 1)
            {
                ViewModel.Notifications.ApplyFilter(NotificationsFilterModel.CreateParticipatingFilter(), true);
            }
            else if (_viewSegment.SelectedSegment == 2)
            {
                ViewModel.Notifications.ApplyFilter(NotificationsFilterModel.CreateAllFilter(), true);
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
            base.ViewWillDisappear(animated);
        }


        private class NotificationSectionView : TableViewSectionView
        {
            UIButton _btn;
            public NotificationSectionView(string s, Action a)
                : base(s)
            {
                Frame = new RectangleF(0, 0, 320, 34);

                if (a != null)
                {
                    _btn = new UIButton();
                    _btn.SetImage(Theme.CurrentTheme.CheckButton, UIControlState.Normal);
                    _btn.TouchUpInside += (sender, e) => a();
                    _btn.Frame = new RectangleF(270, 3, 40, 28);
                    _btn.Layer.ShadowOpacity = 0.6f;
                    _btn.Layer.ShadowColor = UIColor.Black.CGColor;
                    _btn.Layer.ShadowOffset = new SizeF(0, 0);
                    _btn.Layer.MasksToBounds = false;
                    AddSubview(_btn);
                }
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                if (_btn == null)
                {
                    _lbl.Frame = new RectangleF(10, 2, Bounds.Width - 20f, Bounds.Height - 4f);
                }
                else
                {
                    _lbl.Frame = new RectangleF(10, 2, Bounds.Width - 50f, Bounds.Height - 4f);
                    _btn.Frame = new RectangleF(Bounds.Width - 50f, 3, 40f, Bounds.Height - 3f);
                }
            }
        }

    }
}

