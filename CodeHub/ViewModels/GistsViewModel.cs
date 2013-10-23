using System.Collections.Generic;
using GitHubSharp;
using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;
using CodeHub.ViewModels;

namespace CodeHub.ViewModels
{
    public class AccountGistsViewModel : GistsViewModel
    {
        public string Username
        {
            get;
            private set;
        }

        public AccountGistsViewModel(string username)
        {
            Username = username;
        }


        protected override GitHubRequest<List<GistModel>> CreateRequest()
        {
            return Application.Client.Users[Username].Gists.GetGists();
        }
    }

    public class StarredGistsViewModel : GistsViewModel
    {
        protected override GitHubRequest<List<GistModel>> CreateRequest()
        {
            return Application.Client.Gists.GetStarredGists();
        }
    }

    public class PublicGistsViewModel : GistsViewModel
    {
        protected override GitHubRequest<List<GistModel>> CreateRequest()
        {
            return Application.Client.Gists.GetPublicGists();
        }
    }


    public abstract class GistsViewModel : CollectionViewModel<GistModel>, ILoadableViewModel
    {
        protected GistsViewModel()
        {
        }
        
        public async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(CreateRequest(), forceDataRefresh);
        }

        protected abstract GitHubRequest<List<GistModel>> CreateRequest();
    }
}

