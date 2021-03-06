using System;
using CodeHub.Controllers;
using CodeFramework.Views;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch;
using System.Linq;
using CodeFramework.Elements;
using CodeFramework.ViewControllers;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class PullRequestViewController : ViewModelDrivenViewController
    {
        private readonly HeaderView _header;
        private readonly SplitElement _split1;

        public new PullRequestViewModel ViewModel
        {
            get { return (PullRequestViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        public PullRequestViewController(string user, string slug, ulong id)
        {
            Title = "Pull Request #".t() + id;
            ViewModel = new PullRequestViewModel(user, slug, id);

            Root.UnevenRows = true;
            _header = new HeaderView(View.Bounds.Width) { ShadowImage = false };
            _split1 = new SplitElement(new SplitElement.Row { Image1 = Images.Cog, Image2 = Images.Merge }) { BackgroundColor = UIColor.White };

            ViewModel.Bind(x => x.PullRequest, Render);
            ViewModel.BindCollection(x => x.Comments, e => Render());
        }

        public void Render()
        {
            if (ViewModel.PullRequest == null)
                return;

            var root = new RootElement(Title);
            _header.Title = ViewModel.PullRequest.Title;
            _header.Subtitle = "Updated " + (ViewModel.PullRequest.UpdatedAt).ToDaysAgo();
            _header.SetNeedsDisplay();
            root.Add(new Section(_header));

            var secDetails = new Section();
            if (!string.IsNullOrEmpty(ViewModel.PullRequest.Body))
            {
                var desc = new MultilinedElement(ViewModel.PullRequest.Body.Trim()) 
                { 
                    BackgroundColor = UIColor.White,
                    CaptionColor = Theme.CurrentTheme.MainTitleColor, 
                    ValueColor = Theme.CurrentTheme.MainTextColor
                };
                desc.CaptionFont = desc.ValueFont;
                desc.CaptionColor = desc.ValueColor;
                secDetails.Add(desc);
            }

            var merged = (ViewModel.PullRequest.Merged == null || !ViewModel.PullRequest.Merged.Value) ? false : true;

            _split1.Value.Text1 = ViewModel.PullRequest.State;
            _split1.Value.Text2 = merged ? "Merged" : "Not Merged";
            secDetails.Add(_split1);
            root.Add(secDetails);

            root.Add(new Section {
                new StyledStringElement("Commits", () => NavigationController.PushViewController(new ChangesetsViewController(ViewModel.User, ViewModel.Repo, ViewModel.PullRequestId), true), Images.Commit),
                new StyledStringElement("Files", () => NavigationController.PushViewController(new PullRequestFilesViewController(ViewModel.User, ViewModel.Repo, ViewModel.PullRequestId), true), Images.File),
            });

            if (!merged)
            {
                MonoTouch.Foundation.NSAction mergeAction = async () =>
                {
                    try
                    {
                        await this.DoWorkTest("Merging...", () => ViewModel.Merge());
                    }
                    catch (Exception e)
                    {
                        MonoTouch.Utilities.ShowAlert("Unable to Merge", e.Message);
                    }
                };

                if (ViewModel.PullRequest.Mergable == null)
                {
                    var el = new StyledStringElement("Merge".t(), mergeAction, Images.Fork);
                    root.Add(new Section(null, "The mergable state is unknown and merging may not be successful.") { el });
                }
                else if (ViewModel.PullRequest.Mergable.Value)
                {
                    root.Add(new Section { new StyledStringElement("Merge".t(), mergeAction, Images.Fork) });
                }
                else
                {
                    root.Add(new Section { new StyledStringElement("Unable to merge!".t()) { Image = Images.Fork } });
                }
            }


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
//                if (model.MoreComments != null)
//                {
//                    var loadMore = new PaginateElement("Load More".t(), "Loading...".t(), 
//                                                       e => this.DoWorkNoHud(() => model.MoreComments(),
//                                          x => Utilities.ShowAlert("Unable to load more!".t(), x.Message))) { AutoLoadOnVisible = false, Background = false };
//                    commentsSec.Add(loadMore);
//                }

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

