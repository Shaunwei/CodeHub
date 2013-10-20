using GitHubSharp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeFramework.ViewModels;

namespace CodeHub.ViewModels
{
    public class RepositoriesExploreViewModel : CollectionViewModel<RepositorySearchModel.RepositoryModel>
    {
        public bool Searched { get; private set; }

        public async Task Search(string text)
        {
            Searched = true;
            await Task.Run(() => {
                var request = Application.Client.Repositories.SearchRepositories(text);
                request.UseCache = false;
                var response = Application.Client.Execute(request);
                Items.Reset(response.Data.Repositories);
            });
        }

    }
}

