using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using GitHubSharp;
using CodeFramework.Views;
using GitHubSharp.Models;

namespace CodeHub.ViewControllers
{
	public class RawContentViewController : FileSourceViewController
    {
        private readonly string _rawUrl;
        private readonly string _githubUrl;
        private readonly string _filename;
        protected DownloadResult _downloadResult;
        private readonly bool _isBinary;

        public RawContentViewController(string rawUrl, string githubUrl, string filename, bool isBinary)
        {
            _isBinary = isBinary;
            _filename = filename;
            _rawUrl = rawUrl;
            _githubUrl = githubUrl;
            Title = filename.Substring(filename.LastIndexOf('/') + 1);
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(Theme.CurrentTheme.GearButton, ShowExtraMenu));
        }

        private void ShowExtraMenu()
        {
            var sheet = MonoTouch.Utilities.GetSheet(Title);

            var openButton = _downloadResult != null ? sheet.AddButton("Open In".t()) : -1;
            var shareButton = sheet.AddButton("Share".t());
            var showButton = _githubUrl != null ? sheet.AddButton("Show in GitHub".t()) : -1;
            var cancelButton = sheet.AddButton("Cancel".t());
            sheet.CancelButtonIndex = cancelButton;
            sheet.DismissWithClickedButtonIndex(cancelButton, true);
            sheet.Clicked += (s, e) => {
                if (e.ButtonIndex == openButton)
                {
                    var ctrl = new UIDocumentInteractionController();
                    ctrl.Url = NSUrl.FromFilename(_downloadResult.File);
                    ctrl.PresentOpenInMenu(NavigationItem.RightBarButtonItem, true);
                }
                else if (e.ButtonIndex == shareButton)
                {
                    var item = UIActivity.FromObject (_githubUrl);
                    var activityItems = new NSObject[] { item };
                    UIActivity[] applicationActivities = null;
                    var activityController = new UIActivityViewController (activityItems, applicationActivities);
                    PresentViewController (activityController, true, null);
                }
                else if (e.ButtonIndex == showButton)
                {
                    try { UIApplication.SharedApplication.OpenUrl(new NSUrl(_githubUrl)); } catch { }
                }
            };

            sheet.ShowInView(this.View);
        }


        protected override void Request()
        {
            try 
            {
                var result = _downloadResult = DownloadFile2(_rawUrl, _filename);
                var ext = System.IO.Path.GetExtension(_filename).TrimStart('.');
                if (!_isBinary)
                    LoadRawData(System.Security.SecurityElement.Escape(System.IO.File.ReadAllText(result.File, System.Text.Encoding.UTF8)), ext);
                else
                    LoadFile(result.File);
            }
            catch (InternalServerException ex)
            {
                MonoTouch.Utilities.ShowAlert("Error", ex.Message);
            }
        }
    }

    public class SourceInfoViewController : FileSourceViewController
    {
        protected DownloadResult _downloadResult;
        private readonly ContentModel _model;

        public SourceInfoViewController(ContentModel model)
        {
            _model = model;
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(Theme.CurrentTheme.GearButton, ShowExtraMenu));

            //Create the filename
            var fileName = System.IO.Path.GetFileName(model.Path);
            if (fileName == null)
                fileName = model.Path.Substring(model.Path.LastIndexOf('/') + 1);

            //Create the temp file path
            Title = fileName;
        }

        private void ShowExtraMenu()
        {
            var sheet = MonoTouch.Utilities.GetSheet(Title);

            var openButton = _downloadResult != null ? sheet.AddButton("Open In".t()) : -1;
            var shareButton = sheet.AddButton("Share".t());
            var showButton = _model.HtmlUrl != null ? sheet.AddButton("Show in GitHub".t()) : -1;
            var cancelButton = sheet.AddButton("Cancel".t());
            sheet.CancelButtonIndex = cancelButton;
            sheet.DismissWithClickedButtonIndex(cancelButton, true);
            sheet.Clicked += (s, e) => {
                if (e.ButtonIndex == openButton)
                {
                    var ctrl = new UIDocumentInteractionController();
                    ctrl.Url = NSUrl.FromFilename(_downloadResult.File);
                    ctrl.PresentOpenInMenu(NavigationItem.RightBarButtonItem, true);
                }
                else if (e.ButtonIndex == shareButton)
                {
                    var item = UIActivity.FromObject (_model.HtmlUrl);
                    var activityItems = new NSObject[] { item };
                    UIActivity[] applicationActivities = null;
                    var activityController = new UIActivityViewController (activityItems, applicationActivities);
                    PresentViewController (activityController, true, null);
                }
                else if (e.ButtonIndex == showButton)
                {
                    try { UIApplication.SharedApplication.OpenUrl(new NSUrl(_model.HtmlUrl)); } catch { }
                }
            };

            sheet.ShowInView(this.View);
        }

        protected override void Request()
        {
            try 
            {
                var result = _downloadResult = DownloadFile2(_model.GitUrl, _model.Name);
                var ext = System.IO.Path.GetExtension(_model.Name).TrimStart('.');
                if (!result.IsBinary)
                    LoadRawData(System.Security.SecurityElement.Escape(System.IO.File.ReadAllText(result.File, System.Text.Encoding.UTF8)), ext);
                else
                    LoadFile(result.File);
            }
            catch (InternalServerException ex)
            {
                MonoTouch.Utilities.ShowAlert("Error", ex.Message);
            }
        }
    }
}

