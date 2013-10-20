using GitHubSharp.Models;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class RepositoriesStarredViewModel : RepositoriesViewModel
    {
        public RepositoriesStarredViewModel()
            : base(string.Empty)
        {
        }

        public override async Task Load(bool forceDataRefresh)
        {
            await Task.Run(() => this.RequestModel(Application.Client.AuthenticatedUser.Repositories.GetStarred(), forceDataRefresh, response => {
                Items.Reset(response.Data);
                this.CreateMore(response, m => MoreItems = m, d => Items.AddRange(d));
            }));
        }
    }
}

