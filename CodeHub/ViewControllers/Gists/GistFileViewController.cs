using GitHubSharp.Models;
using MonoTouch.UIKit;
using CodeFramework.Views;
using CodeFramework.ViewControllers;
using GitHubSharp;

namespace CodeHub.ViewControllers
{
    public class GistFileViewController : RawContentViewController
    {
        GistFileModel _model;
        private string _content;

        public GistFileViewController(GistFileModel model, string gistUrl, string content = null)
            : base(model.RawUrl, gistUrl, model.Filename, false)
        {
            _model = model;
            Title = model.Filename;
            _content = content;
        }

        protected override void Request()
        {
            if (_content == null)
            {
                try 
                {
                    var result = _downloadResult = DownloadFile(_model.RawUrl);
                    var ext = System.IO.Path.GetExtension(_model.RawUrl).TrimStart('.');
                    LoadRawData(System.Security.SecurityElement.Escape(System.IO.File.ReadAllText(result.File, System.Text.Encoding.UTF8)), ext);
                }
                catch (InternalServerException ex)
                {
                    MonoTouch.Utilities.ShowAlert("Error", ex.Message);
                }
            }
            else
            {
                var filePath = CreateFile(_model.Filename);
                System.IO.File.WriteAllText(filePath, _content, System.Text.Encoding.UTF8);
                _downloadResult = new DownloadResult { File = filePath, IsBinary = false };
                var ext = System.IO.Path.GetExtension(_model.Filename).TrimStart('.');
                LoadRawData(System.Security.SecurityElement.Escape(_content), ext);
            }
        }
    }

    public class GistViewableFileController : WebViewController
    {
        GistFileModel _model;
        string _rawContent;
        bool _loaded = false;

        public GistViewableFileController(GistFileModel model, string gistUrl)
            : base(true)
        {
            _model = model;
            Title = model.Filename;

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(Theme.CurrentTheme.ViewButton, () => {
                NavigationController.PushViewController(new GistFileViewController(model, gistUrl, _rawContent), true);
            }));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //Do the request
            if (_loaded == false)
            {
                this.DoWork(() => {
                    string data = _rawContent;
                    if (data == null)
                    {
                        using (var ms = new System.IO.MemoryStream())
                        {
                            Application.Client.DownloadRawResource(_model.RawUrl, ms);
                            ms.Position = 0;
                            var sr = new System.IO.StreamReader(ms);
                            data = _rawContent = sr.ReadToEnd();
                        }
                    }
                    if (_model.Language.Equals("Markdown"))
                        data = Application.Client.Markdown.GetMarkdown(data);

                    var path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetTempFileName() + ".html");
                    System.IO.File.WriteAllText(path, data, System.Text.Encoding.UTF8);
                    LoadFile(path);
                    _loaded = true;
                });
            }
        }

        protected override void OnLoadError(object sender, UIWebErrorArgs e)
        {
            base.OnLoadError(sender, e);

            //Can't load this!
            MonoTouch.Utilities.ShowAlert("Error", "Unable to display this type of file.");
        }

        protected void LoadRawData(string data)
        {
            InvokeOnMainThread(delegate {
                Web.LoadHtmlString(data, null);
            });
        }
    }
}

