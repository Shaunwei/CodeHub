using System;
using CodeHub.Controllers;
using CodeFramework.Views;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch;
using System.Collections.Generic;
using System.Linq;
using CodeFramework.Elements;
using MonoTouch.Foundation;
using GitHubSharp.Models;
using CodeFramework.ViewControllers;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class IssueViewController : ViewModelDrivenViewController
    {
        private readonly HeaderView _header;
        private readonly SplitElement _split1;

        private bool _issueRemoved;

        public new IssueViewModel ViewModel
        {
            get { return (IssueViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        public IssueViewController(string user, string slug, ulong id)
        {
            Title = "Issue #" + id;
            ViewModel = new IssueViewModel(user, slug, id);

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(Theme.CurrentTheme.EditButton, () => {
                var editController = new IssueEditViewController(ViewModel.Username, ViewModel.Repository) {
                    ExistingIssue = ViewModel.Issue,
                    Title = "Edit Issue",
                    Success = EditingComplete,
                };
                NavigationController.PushViewController(editController, true);
            }));
            NavigationItem.RightBarButtonItem.Enabled = false;

            Style = UITableViewStyle.Grouped;
            Root.UnevenRows = true;
            _header = new HeaderView(View.Bounds.Width) { ShadowImage = false };
            _split1 = new SplitElement(new SplitElement.Row { Image1 = Images.Cog, Image2 = Images.Milestone }) { BackgroundColor = UIColor.White };

            ViewModel.Bind(x => x.Issue, Render);
            ViewModel.BindCollection(x => x.Comments, (e) => Render());
        }

        public void Render()
        {
            //This means we've deleted it. Due to the code flow, render will get called after the update, regardless.
            if (ViewModel.Issue == null)
                return;

            //We've loaded, we can edit
            NavigationItem.RightBarButtonItem.Enabled = true;

            var root = new RootElement(Title);
            _header.Title = ViewModel.Issue.Title;
            _header.Subtitle = "Updated " + (ViewModel.Issue.UpdatedAt).ToDaysAgo();
            _header.SetNeedsDisplay();
            root.Add(new Section(_header));

            var secDetails = new Section();
            if (!string.IsNullOrEmpty(ViewModel.Issue.Body))
            {
                var desc = new MultilinedElement(ViewModel.Issue.Body.Trim()) 
                { 
                    BackgroundColor = UIColor.White,
                    CaptionColor = Theme.CurrentTheme.MainTitleColor, 
                    ValueColor = Theme.CurrentTheme.MainTextColor
                };
                desc.CaptionFont = desc.ValueFont;
                desc.CaptionColor = desc.ValueColor;
                secDetails.Add(desc);
            }

            _split1.Value.Text1 = ViewModel.Issue.State;
            _split1.Value.Text2 = ViewModel.Issue.Milestone == null ? "No Milestone".t() : ViewModel.Issue.Milestone.Title;
            secDetails.Add(_split1);

            var responsible = new StyledStringElement(ViewModel.Issue.Assignee != null ? ViewModel.Issue.Assignee.Login : "Unassigned".t()) {
                Font = StyledStringElement.DefaultDetailFont,
                TextColor = StyledStringElement.DefaultDetailColor,
                Image = Images.Person
            };

            if (ViewModel.Issue.Assignee != null)
            {
                responsible.Tapped += () => NavigationController.PushViewController(new ProfileViewController(ViewModel.Issue.Assignee.Login), true);
                responsible.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            }

            secDetails.Add(responsible);
            root.Add(secDetails);

            if (ViewModel.Comments.Items.Count > 0)
            {
                var commentsSec = new Section();
                ViewModel.Comments.OrderBy(x => (x.CreatedAt)).ToList().ForEach(x => {
                    if (!string.IsNullOrEmpty(x.Body))
                        commentsSec.Add(new CommentElement {
                            Name = x.User.Login,
                            Time = x.CreatedAt.ToDaysAgo(),
                            String = x.Body,
                            Image = Theme.CurrentTheme.AnonymousUserImage,
                            ImageUri = new Uri(x.User.AvatarUrl),
                            BackgroundColor = UIColor.White,
                        });
                });

                //Load more if there's more comments
                if (ViewModel.Comments.MoreItems != null)
                {
                    var loadMore = new PaginateElement("Load More".t(), "Loading...".t(), 
                                                       e => this.DoWorkNoHud(() => ViewModel.Comments.MoreItems(),
                                                       x => Utilities.ShowAlert("Unable to load more!".t(), x.Message))) { AutoLoadOnVisible = false, Background = false };
                    commentsSec.Add(loadMore);
                }

                if (commentsSec.Elements.Count > 0)
                    root.Add(commentsSec);
            }

            
            var addComment = new StyledStringElement("Add Comment") { Image = Images.Pencil };
            addComment.Tapped += AddCommentTapped;
            root.Add(new Section { addComment });
            Root = root;

//            if (_scrollToLastComment && _comments.Elements.Count > 0)
//            {
//                TableView.ScrollToRow(NSIndexPath.FromRowSection(_comments.Elements.Count - 1, 2), UITableViewScrollPosition.Top, true);
//                _scrollToLastComment = false;
//            }
        }

        void EditingComplete(IssueModel model)
        {
            ViewModel.Issue = model;

            //If it's null then we've deleted it!
            if (model == null)
                _issueRemoved = true;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (_issueRemoved)
                NavigationController.PopViewControllerAnimated(true);
        }

        void AddCommentTapped()
        {
            var composer = new Composer();
            composer.NewComment(this, (text) => {
                try
                {
                    composer.DoWorkTest("Loading...".t(), async () => {
                        await ViewModel.AddComment(text);
                        composer.CloseComposer();
                    });
                }
                catch (Exception ex)
                {
                    Utilities.ShowAlert("Unable to post comment!", ex.Message);
                }
                finally
                {
                    composer.EnableSendButton = true;
                }
            });
        }

        public override UIView InputAccessoryView
        {
            get
            {
                var u = new UIView(new System.Drawing.RectangleF(0, 0, 320f, 27)) { BackgroundColor = UIColor.White };
                return u;
            }
        }
    }
}

