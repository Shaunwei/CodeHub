using CodeFramework.ViewControllers;
using CodeFramework.Views;
using MonoTouch.UIKit;
using CodeHub.ViewModels;
using GitHubSharp.Models;
using MonoTouch.Foundation;
using System;
using MonoTouch.Dialog;

namespace CodeHub.ViewControllers
{
    public class GistViewController : ViewModelDrivenViewController
    {
        private readonly HeaderView _header;
        private readonly UIBarButtonItem _shareButton, _userButton;
        UIButton _starButton;

        public new GistViewModel ViewModel
        {
            get { return (GistViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public GistViewController(string id)
        {
            Title = "Gist";
            ViewModel = new GistViewModel(id);
            _header = new HeaderView(0f) { Title = "Gist: " + id };

            ToolbarItems = new []
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                new UIBarButtonItem((_starButton = ToolbarButton.Create(Images.Gist.Star, StarButtonPress))),
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                (_userButton = new UIBarButtonItem(ToolbarButton.Create(Images.Gist.User, UserButtonPress))),
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                (_shareButton = new UIBarButtonItem(ToolbarButton.Create(Images.Gist.Share, ShareButtonPress))),
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
            };

            //Disable these buttons until the gist object becomes valid
            _userButton.Enabled = false;
            _shareButton.Enabled = false;

            this.Bind(ViewModel, x => x.Gist, (gist) => {
                UpdateOwned();
                RenderGist(gist);
            });
            this.Bind(ViewModel, x => x.IsStarred, isStarred => {
                _starButton.SetImage(isStarred ? Images.Gist.StarHighlighted : Images.Gist.Star, UIControlState.Normal);
                _starButton.SetNeedsDisplay();
            });
        }

        public GistViewController(GistModel model)
            : this (model.Id)
        {
            //Controller.Model.Gist = model;
        }
        
        private void UpdateOwned()
        {
            //Is it owned?
            if (string.Equals(Application.Account.Username, ViewModel.Gist.User.Login, StringComparison.OrdinalIgnoreCase))
            {
                NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(CodeFramework.Theme.CurrentTheme.EditButton, () => {
                    //We need to make sure we have the FULL gist
                    this.DoWork(() => {
                        var gist = Application.Client.Execute(Application.Client.Gists[ViewModel.Id].Get()).Data;
                        InvokeOnMainThread(() => {
                            var gistController = new EditGistController(gist);
                            gistController.Created = (editedGist) => {
                                ViewModel.Gist = editedGist;
                            };
                            var navController = new UINavigationController(gistController);
                            PresentViewController(navController, true, null);
                        });
                    });
                }));
            }
            else
            {
                NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(CodeFramework.Theme.CurrentTheme.ForkButton, () => {
                    NavigationItem.RightBarButtonItem.Enabled = false;

                    this.DoWork(() => {
                        var forkedGist = Application.Client.Execute(Application.Client.Gists[ViewModel.Id].ForkGist()).Data;
                        InvokeOnMainThread(delegate {
                            NavigationController.PushViewController(new GistViewController(forkedGist), true);
                        });
                    }, null, () => {
                        NavigationItem.RightBarButtonItem.Enabled = true;
                    });
                }));
            }
        }
        
        private void ShareButtonPress()
        {
            if (ViewModel.Gist == null)
                return;

            var sheet = MonoTouch.Utilities.GetSheet("Gist");

            var shareButton = sheet.AddButton("Share".t());
            var showButton = sheet.AddButton("Show in GitHub".t());
            var cancelButton = sheet.AddButton("Cancel".t());
            sheet.CancelButtonIndex = cancelButton;
            sheet.DismissWithClickedButtonIndex(cancelButton, true);
            sheet.Clicked += (s, e) => {
                if (e.ButtonIndex == shareButton)
                {
                    var item = UIActivity.FromObject (ViewModel.Gist.HtmlUrl);
                    var activityItems = new NSObject[] { item };
                    UIActivity[] applicationActivities = null;
                    var activityController = new UIActivityViewController (activityItems, applicationActivities);
                    PresentViewController (activityController, true, null);
                }
                else if (e.ButtonIndex == showButton)
                {
                    try { UIApplication.SharedApplication.OpenUrl(new NSUrl(ViewModel.Gist.HtmlUrl)); } catch { }
                }
            };

            sheet.ShowFrom(_shareButton, true);
        }

        private void UserButtonPress()
        {
            if (ViewModel.Gist != null)
                NavigationController.PushViewController(new ProfileViewController(ViewModel.Gist.User.Login), true);
        }

        private async void StarButtonPress()
        {
            try
            {
                await ViewModel.SetStarred(!ViewModel.IsStarred);
            }
            catch (Exception e)
            {
                MonoTouch.Utilities.LogException("Unable to star", e);
                MonoTouch.Utilities.ShowAlert("Error".t(), e.Message);
            }
        }

        public void RenderGist(GistModel model)
        {
            _shareButton.Enabled = _userButton.Enabled = model != null;
            var root = new RootElement(Title) { UnevenRows = true };
            var sec = new Section();
            _header.Subtitle = "Updated " + model.UpdatedAt.ToDaysAgo();


            var str = string.IsNullOrEmpty(model.Description) ? "Gist " + model.Id : model.Description;
            var d = new NameTimeStringElement() { 
                Time = model.UpdatedAt.ToDaysAgo(), 
                String = str, 
                Image = Theme.CurrentTheme.AnonymousUserImage,
                BackgroundColor = UIColor.White,
                UseBackgroundColor = true,
            };

            //Sometimes there's no user!
            d.Name = (model.User == null) ? "Anonymous" : model.User.Login;
            d.ImageUri = (model.User == null) ? null : new Uri(model.User.AvatarUrl);

            sec.Add(d);

            var sec2 = new Section();

            foreach (var file in model.Files.Keys)
            {
                var sse = new StyledStringElement(file, model.Files[file].Size + " bytes", UITableViewCellStyle.Subtitle) { 
                    Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                    LineBreakMode = MonoTouch.UIKit.UILineBreakMode.TailTruncation,
                    Lines = 1 
                };

                var fileSaved = file;
                var gistFileModel = model.Files[fileSaved];

                if (string.Equals(gistFileModel.Language, "markdown", StringComparison.OrdinalIgnoreCase))
                    sse.Tapped += () => NavigationController.PushViewController(new GistViewableFileController(gistFileModel, ViewModel.Gist.HtmlUrl), true);
                else
                    sse.Tapped += () => NavigationController.PushViewController(new GistFileViewController(gistFileModel, ViewModel.Gist.HtmlUrl), true);

                sec2.Add(sse);
            }

            _header.SetNeedsDisplay();
            root.Add(new [] { sec, sec2 });
            Root = root;
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

