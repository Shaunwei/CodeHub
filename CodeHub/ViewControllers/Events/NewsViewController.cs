using CodeHub.Controllers;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class NewsViewController : BaseEventsViewController
    {
        public NewsViewController()
            : base(new NewsViewModel())
        {
            Title = "News".t();
        }
    }
}

