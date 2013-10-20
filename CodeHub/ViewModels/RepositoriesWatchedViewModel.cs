using GitHubSharp.Models;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class RepositoriesWatchedViewModel : RepositoriesViewModel
    {
        public RepositoriesWatchedViewModel()
            : base(string.Empty)
        {
        }

        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.AuthenticatedUser.Repositories.GetWatching(), forceDataRefresh);
        }
    }
}

