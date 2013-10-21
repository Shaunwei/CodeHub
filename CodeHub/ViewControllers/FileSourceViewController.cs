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
        private static string[] BinaryMIMEs = new string[] 
        { 
            "image/", "video/", "audio/", "model/", "application/pdf", "application/zip", "application/gzip"
        };

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
            string mime = null;

            //Find
            using (var stream = new System.IO.FileStream(filepath, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
                mime = Application.Client.DownloadRawResource(rawUrl, stream) ?? string.Empty;
            }

            return new DownloadResult { IsBinary = IsBinary(mime), File = filepath };
        }

        private static bool IsBinary(string mime)
        {
            var lowerMime = mime.ToLower();
            foreach (var m in BinaryMIMEs)
            {
                if (lowerMime.StartsWith(m))
                    return true;
            }

            return false;
        }
    }
}

