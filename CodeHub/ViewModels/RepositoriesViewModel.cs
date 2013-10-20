using GitHubSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using CodeHub.Filters.Models;
using System.Threading.Tasks;
using GitHubSharp;
using CodeFramework.ViewModels;
using CodeFramework.Utils;

namespace CodeHub.ViewModels
{
    public class RepositoriesViewModel : FilterableCollectionViewModel<RepositoryModel, RepositoriesFilterModel>, ILoadableViewModel
    {
        public string Username 
        { 
            get; 
            private set; 
        }

        public RepositoriesViewModel(string username, string filterKey = "RepositoryController")
            : base(filterKey)
        {
            Username = username;
            GroupingFunction = CreateGroupedItems;
        }

        public virtual async Task Load(bool forceDataRefresh)
        {
            GitHubRequest<List<RepositoryModel>> request;
            if (string.Equals(Application.Account.Username, Username, StringComparison.OrdinalIgnoreCase))
                request = Application.Client.AuthenticatedUser.Repositories.GetAll();
            else
                request = Application.Client.Users[Username].Repositories.GetAll();

            await Task.Run(() => this.RequestModel(request, forceDataRefresh, response => {
                Items.Reset(FilterModel(response.Data));
                this.CreateMore(response, m => MoreItems = m, d => Items.AddRange(FilterModel(d)));
            }));
        }

        protected override void FilterChanged()
        {
            Items.Reset(FilterModel(Items).ToList());
        }

        private IEnumerable<RepositoryModel> FilterModel(IEnumerable<RepositoryModel> model)
        {
            return (Filter.Ascending ? model.OrderBy(x => x.Name) : model.OrderByDescending(x => x.Name));
        }

        private IEnumerable<IGrouping<string, RepositoryModel>> CreateGroupedItems(IEnumerable<RepositoryModel> model)
        {
            var order = (RepositoriesFilterModel.Order)Filter.OrderBy;
            if (order == RepositoriesFilterModel.Order.Forks)
            {
                var a = model.OrderBy(x => x.Forks).GroupBy(x => IntegerCeilings.First(r => r > x.Forks));
                a = Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return CreateNumberedGroup(a, "Forks");
            }
            if (order == RepositoriesFilterModel.Order.LastUpdated)
            {
                var a = model.OrderByDescending(x => x.UpdatedAt).GroupBy(x => IntegerCeilings.First(r => r > x.UpdatedAt.TotalDaysAgo()));
                a = Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return CreateNumberedGroup(a, "Days Ago", "Updated");
            }
            if (order == RepositoriesFilterModel.Order.CreatedOn)
            {
                var a = model.OrderByDescending(x => x.CreatedAt).GroupBy(x => IntegerCeilings.First(r => r > x.CreatedAt.TotalDaysAgo()));
                a = Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return CreateNumberedGroup(a, "Days Ago", "Created");
            }
            if (order == RepositoriesFilterModel.Order.Followers)
            {
                var a = model.OrderBy(x => x.Watchers).GroupBy(x => IntegerCeilings.First(r => r > x.Watchers));
                a = Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return CreateNumberedGroup(a, "Followers");
            }
            if (order == RepositoriesFilterModel.Order.Owner)
            {
                var a = model.OrderBy(x => x.Name).GroupBy(x => x.Owner.Login);
                a = Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return a.ToList();
            }

            return null;
        }
    }
}