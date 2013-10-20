using System.Collections.Generic;
using System.Linq;
using CodeHub.Filters.Models;
using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;
using CodeHub.ViewModels;

namespace CodeHub.Controllers
{
    public class SourceViewModel : FilterableCollectionViewModel<ContentModel, SourceFilterModel>, ILoadableViewModel
    {
        public string Username
        {
            get;
            private set;
        }

        public string Path
        {
            get;
            private set;
        }

        public string Branch
        {
            get;
            private set;
        }

        public string Repository
        {
            get;
            private set;
        }

        public SourceViewModel(string username, string slug, string branch = "master", string path = "")
            : base("SourceViewModel")
        {
            Username = username;
            Repository = slug;
            Branch = branch;
            Path = path;
        }

        protected override void FilterChanged()
        {
            Items.Reset(FilterModel(Items.ToList()));
        }

        protected IEnumerable<ContentModel> FilterModel(IEnumerable<ContentModel> model)
        {
            IEnumerable<ContentModel> ret = model;
            var order = (SourceFilterModel.Order)_filter.OrderBy;
            if (order == SourceFilterModel.Order.Alphabetical)
                ret = model.OrderBy(x => x.Name);
            else if (order == SourceFilterModel.Order.FoldersThenFiles)
                ret = model.OrderBy(x => x.Type).ThenBy(x => x.Name);
            return _filter.Ascending ? ret : ret.Reverse();
        }

        public async Task Load(bool forceDataRefresh)
        {
            await Task.Run(() => this.RequestModel(Application.Client.Users[Username].Repositories[Repository].GetContent(Path, Branch), forceDataRefresh, response => {
                Items.Reset(FilterModel(response.Data));
                this.CreateMore(response, m => MoreItems = m, d => {
                    var current = Items.ToList();
                    current.AddRange(d);
                    Items.Reset(FilterModel(current));
                });
            }));
        }
    }
}

