using System;
using GitHubSharp.Models;
using MonoTouch.Dialog;
using System.Linq;
using CodeHub.Controllers;
using CodeFramework.Views;
using CodeFramework.Elements;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch;
using MonoTouch.Foundation;
using CodeHub.ViewControllers;
using CodeFramework.ViewControllers;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class ChangesetInfoViewController : ViewModelDrivenViewController
    {
        private readonly HeaderView _header;
        private readonly UISegmentedControl _viewSegment;
        private readonly UIBarButtonItem _segmentBarButton;
        private readonly Section _commentsSection;

        public CodeHub.Utils.RepositoryIdentifier Repo { get; set; }

        public new ChangesetInfoViewModel ViewModel 
        {
            get { return (ChangesetInfoViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }
        
        public ChangesetInfoViewController(string user, string repository, string node)
        {
            Title = "Commit".t();
            Root.UnevenRows = true;
            ViewModel = new ChangesetInfoViewModel(user, repository, node);
            
            _header = new HeaderView(0f) { Title = "Commit: ".t() + node.Substring(0, node.Length > 10 ? 10 : node.Length) };
            _viewSegment = new UISegmentedControl(new string[] { "Changes".t(), "Comments".t() });
            _viewSegment.ControlStyle = UISegmentedControlStyle.Bar;
            _segmentBarButton = new UIBarButtonItem(_viewSegment);
            _commentsSection = new Section();

            Bind(ViewModel, x => x.Changeset, Render);
            BindCollection(ViewModel, x => x.Comments, (a) => Render());
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Fucking bug in the divider
            BeginInvokeOnMainThread(delegate {
                _viewSegment.SelectedSegment = 1;
                _viewSegment.SelectedSegment = 0;
                _viewSegment.ValueChanged += (sender, e) => Render();
            });

            _segmentBarButton.Width = View.Frame.Width - 10f;
            ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
        }

        public void Render()
        {
            var commitModel = ViewModel.Changeset;
            if (commitModel == null)
                return;

            var root = new RootElement(Title) { UnevenRows = Root.UnevenRows };

            _header.Subtitle = "Commited ".t() + (commitModel.Commit.Committer.Date).ToDaysAgo();
            var headerSection = new Section(_header);
            root.Add(headerSection);

            var detailSection = new Section();
            root.Add(detailSection);

            var user = "Unknown";
            if (commitModel.Author != null)
                user = commitModel.Author.Login;
            if (commitModel.Commit.Author != null)
                user = commitModel.Commit.Author.Name;

            detailSection.Add(new MultilinedElement(user, commitModel.Commit.Message)
            {
                CaptionColor = Theme.CurrentTheme.MainTextColor,
                ValueColor = Theme.CurrentTheme.MainTextColor,
                BackgroundColor = UIColor.White
            });

            if (Repo != null)
            {
                var repo = new StyledStringElement(Repo.Name) { 
                    Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                    Lines = 1, 
                    Font = StyledStringElement.DefaultDetailFont, 
                    TextColor = StyledStringElement.DefaultDetailColor,
                    Image = Images.Repo
                };
                repo.Tapped += () => NavigationController.PushViewController(new RepositoryViewController(Repo.Owner, Repo.Name), true);
                detailSection.Add(repo);
            }

            if (_viewSegment.SelectedSegment == 0)
            {
                var fileSection = new Section();
                commitModel.Files.ForEach(x => {
                    var file = x.Filename.Substring(x.Filename.LastIndexOf('/') + 1);
                    var sse = new ChangesetElement(file, x.Status, x.Additions, x.Deletions);
                    sse.Tapped += () => {
                        string parent = null;
                        if (commitModel.Parents != null && commitModel.Parents.Count > 0)
                            parent = commitModel.Parents[0].Sha;

                        NavigationController.PushViewController(new ChangesetDiffViewController(ViewModel.User, ViewModel.Repository, commitModel.Sha, x) { Comments = ViewModel.Comments.Items.ToList() }, true);
                    };
                    fileSection.Add(sse);
                });

                if (fileSection.Elements.Count > 0)
                    root.Add(fileSection);
            }
            else if (_viewSegment.SelectedSegment == 1)
            {
                var commentSection = new Section();
                foreach (var comment in ViewModel.Comments)
                {
                    //The path should be empty to indicate it's a comment on the entire commit, not a specific file
                    if (!string.IsNullOrEmpty(comment.Path))
                        continue;

                    commentSection.Add(new CommentElement {
                        Name = comment.User.Login,
                        Time = comment.CreatedAt.ToDaysAgo(),
                        String = comment.Body,
                        Image = Images.Anonymous,
                        ImageUri = new Uri(comment.User.AvatarUrl),
                        BackgroundColor = UIColor.White,
                    });
                }

                if (commentSection.Elements.Count > 0)
                    root.Add(commentSection);

                var addComment = new StyledStringElement("Add Comment".t()) { Image = Images.Pencil };
                addComment.Tapped += AddCommentTapped;
                root.Add(new Section { addComment });
            }

            Root = root; 
        }

        void AddCommentTapped()
        {
            var composer = new Composer();
            composer.NewComment(this, (text) => {
                try
                {
                    composer.DoWorkTest("Commenting...".t(), async () => {
                        await ViewModel.AddComment(text);
                        composer.CloseComposer();
                    });
                }
                catch (Exception e)
                {
                    Utilities.ShowAlert("Unable to post comment!", e.Message);
                }
                finally
                {
                    composer.EnableSendButton = true;
                }
            });
        }

        public override void ViewWillAppear(bool animated)
        {
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(false, animated);
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
        }
    }
}

