using System;
using CodeHub.Controllers;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch;
using CodeFramework.Views;
using CodeFramework.ViewControllers;
using System.Threading.Tasks;

namespace CodeHub.ViewControllers
{
    public class ReadmeViewController : WebViewController
    {
        private readonly string _user;
        private readonly string _slug;
        private ErrorView _errorView;
        private bool _isVisible;
        private bool _isLoaded;
        private string _tmpUri; 

        public ReadmeViewController(string user, string slug)
            : base(true)
        {
            _user = user;
            _slug = slug;
            Title = "Readme";
            Web.ScalesPageToFit = true;
        }

        private string RequestAndSave(bool forceInvalidation)
        {
            var wiki = Application.Client.Execute(Application.Client.Users[_user].Repositories[_slug].GetReadme()).Data;
            var d = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(wiki.Content));
            var data = Application.Client.Markdown.GetMarkdown(d);

            //Generate the markup
            var markup = new System.Text.StringBuilder();
            markup.Append("<html><head>");
            markup.Append("<meta name=\"viewport\" content=\"width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=0\"/>");
            markup.Append("<title>Readme");
            markup.Append("</title></head><body>");
            markup.Append(data);
            markup.Append("</body></html>");

            var tmp = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetTempFileName() + ".html");
            System.IO.File.WriteAllText(tmp, markup.ToString(), System.Text.Encoding.UTF8);
            return tmp;
        }
        
        private void Load(bool forceInvalidation = false)
        {
            this.DoWork(() => {
                if (_errorView != null)
                {
                    InvokeOnMainThread(delegate {
                        _errorView.RemoveFromSuperview();
                        _errorView = null;
                    });
                }
                
                _tmpUri = LoadFile(RequestAndSave(forceInvalidation));
            },
            ex => {
                if (_isVisible)
                    Utilities.ShowAlert("Unable to Find Wiki Page", ex.Message);
            });

        }
        
        public override void ViewDidDisappear(bool animated)
        {
            _isVisible = false;
            base.ViewDidDisappear(animated);
        }
        
        public override void ViewDidAppear(bool animated)
        {
            _isVisible = true;
            base.ViewDidAppear(animated);
            
            //Load the page
            if (!_isLoaded)
                Load();
            _isLoaded = true;
        }

        protected override async void Refresh()
        {
            if (Web.Request.Url.AbsoluteString.Equals(_tmpUri))
            {
                if (RefreshButton != null)
                    RefreshButton.Enabled = false;

                try
                {
                    await this.DoWorkNoHudAsync(async () => {
                        await Task.Run(() => RequestAndSave(true));
                    });
                }
                catch (Exception e)
                {
                    if (_isVisible)
                        Utilities.ShowAlert("Unable to Refresh!", e.Message);
                    if (RefreshButton != null)
                        RefreshButton.Enabled = true;
                    return;
                }
            }


            base.Refresh();
        }
    }
}

