using GitHubSharp.Models;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class RepoEventsViewController : BaseEventsViewController
    {
        public RepoEventsViewController(string username, string slug)
            : base(new RepositoryEventsViewModel(username, slug))
        {
            ReportRepository = false;
        }
    }
}

