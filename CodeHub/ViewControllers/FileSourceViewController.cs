using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CodeHub.Controllers;
using System.Text;
using CodeFramework.Views;

namespace CodeHub.ViewControllers
{
    public abstract class FileSourceViewController : CodeFramework.ViewControllers.FileSourceViewController
    {
        public class DownloadResult
        {
            public string File { get; set; }
            public bool IsBinary { get; set; }
        }

        protected static string CreateFile(string filename)
        {
            var ext = System.IO.Path.GetExtension(filename);
            if (ext == null) ext = string.Empty;
            var newFilename = Environment.TickCount + ext;
            return System.IO.Path.Combine(TempDir, newFilename);
        }

        protected static DownloadResult DownloadFile(string rawUrl)
        {
            //Create a temporary filename
            var filepath = CreateFile(rawUrl);
            var result = new DownloadResult();

            //Find
            using (var stream = new System.IO.FileStream(filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                var mime = Application.Client.DownloadRawResource(rawUrl, stream);
                if (mime.Contains("text") && mime.Contains("charset"))
                    result.IsBinary = false;
                else
                    result.IsBinary = true;
            }

            result.File = filepath;
            return result;
        }
    }
}

