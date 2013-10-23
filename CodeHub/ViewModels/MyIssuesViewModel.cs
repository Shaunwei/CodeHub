using System;
using GitHubSharp.Models;
using CodeHub.Filters.Models;
using System.Collections.Generic;
using System.Linq;
using CodeFramework.ViewModels;
using System.Threading.Tasks;
using CodeHub.ViewModels;
using CodeFramework.Utils;

namespace CodeHub.ViewModels
{
    public class MyIssuesViewModel : FilterableCollectionViewModel<IssueModel, MyIssuesFilterModel>, ILoadableViewModel
    {
        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            protected set { SetProperty(ref _isLoading, value); }
        }

        public MyIssuesViewModel()
            : base("MyIssues")
        {
            GroupingFunction = Group;
        }

        public async Task Load(bool forceDataRefresh)
        {
            string filter = Filter.FilterType.ToString().ToLower();
            string direction = Filter.Ascending ? "asc" : "desc";
            string state = Filter.Open ? "open" : "closed";
            string sort = Filter.SortType == MyIssuesFilterModel.Sort.None ? null : Filter.SortType.ToString().ToLower();
            string labels = string.IsNullOrEmpty(Filter.Labels) ? null : Filter.Labels;

            var request = Application.Client.AuthenticatedUser.Issues.GetAll(sort: sort, labels: labels, state: state, direction: direction, filter: filter);
            await Task.Run(() => this.RequestModel(request, forceDataRefresh, response => {
                Items.Reset(response.Data);
                this.CreateMore(response, m => MoreItems = m, d => Items.AddRange(d));
            }));
        }
        
        private List<IGrouping<string, IssueModel>> Group(IEnumerable<IssueModel> model)
        {
            var order = Filter.SortType;
            if (order == MyIssuesFilterModel.Sort.Comments)
            {
                var a = Filter.Ascending ? model.OrderBy(x => x.Comments) : model.OrderByDescending(x => x.Comments);
                var g = a.GroupBy(x => IntegerCeilings.First(r => r > x.Comments)).ToList();
                return CreateNumberedGroup(g, "Comments");
            }
            else if (order == MyIssuesFilterModel.Sort.Updated)
            {
                var a = Filter.Ascending ? model.OrderBy(x => x.UpdatedAt) : model.OrderByDescending(x => x.UpdatedAt);
                var g = a.GroupBy(x => IntegerCeilings.First(r => r > x.UpdatedAt.TotalDaysAgo()));
                return CreateNumberedGroup(g, "Days Ago", "Updated");
            }
            else if (order == MyIssuesFilterModel.Sort.Created)
            {
                var a = Filter.Ascending ? model.OrderBy(x => x.CreatedAt) : model.OrderByDescending(x => x.CreatedAt);
                var g = a.GroupBy(x => IntegerCeilings.First(r => r > x.CreatedAt.TotalDaysAgo()));
                return CreateNumberedGroup(g, "Days Ago", "Created");
            }

            return null;
        }

        protected override async void FilterChanged()
        {
            IsLoading = true;

            try
            {
                await Load(true);
            }
            catch (Exception e)
            {
                //Do nothing...
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}

