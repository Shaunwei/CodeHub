using System.Collections.Generic;
using System.Linq;
using CodeHub.Filters.Models;
using GitHubSharp.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;
using CodeHub.ViewModels;

namespace CodeHub.Controllers
{
    public class SourceViewModel : ViewModel, ILoadableViewModel
    {
        private readonly FilterableCollectionViewModel<ContentModel, SourceFilterModel> _content;

        public FilterableCollectionViewModel<ContentModel, SourceFilterModel> Content
        {
            get { return _content; }
        }

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
        {
            Username = username;
            Repository = slug;
            Branch = branch;
            Path = path;

            _content = new FilterableCollectionViewModel<ContentModel, SourceFilterModel>("SourceViewModel");
            _content.FilteringFunction = FilterModel;
            _content.Bind(x => x.Filter, () => _content.Refresh());
        }

        private IEnumerable<ContentModel> FilterModel(IEnumerable<ContentModel> model)
        {
            IEnumerable<ContentModel> ret = model;
            var order = (SourceFilterModel.Order)_content.Filter.OrderBy;
            if (order == SourceFilterModel.Order.Alphabetical)
                ret = model.OrderBy(x => x.Name);
            else if (order == SourceFilterModel.Order.FoldersThenFiles)
                ret = model.OrderBy(x => x.Type).ThenBy(x => x.Name);
            return _content.Filter.Ascending ? ret : ret.Reverse();
        }

        public Task Load(bool forceDataRefresh)
        {
            return Content.SimpleCollectionLoad(Application.Client.Users[Username].Repositories[Repository].GetContent(Path, Branch), forceDataRefresh);
        }
    }
}

