using GitHubSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using CodeHub.Filters.Models;
using System.Threading.Tasks;
using GitHubSharp;
using CodeFramework.ViewModels;
using CodeFramework.Utils;
using CodeFramework;

namespace CodeHub.ViewModels
{
    public class RepositoriesViewModel : ViewModel, ILoadableViewModel
    {
        private readonly FilterableCollectionViewModel<RepositoryModel, RepositoriesFilterModel> _repositories;

        public FilterableCollectionViewModel<RepositoryModel, RepositoriesFilterModel> Repositories
        {
            get { return _repositories; }
        }

        public string Username 
        { 
            get; 
            private set; 
        }

        public RepositoriesViewModel(string username, string filterKey = "RepositoryController")
        {
            Username = username;

            _repositories = new FilterableCollectionViewModel<RepositoryModel, RepositoriesFilterModel>(filterKey);
            _repositories.FilteringFunction = x => _repositories.Filter.Ascending ? x.OrderBy(y => y.Name) : x.OrderByDescending(y => y.Name);
            _repositories.GroupingFunction = CreateGroupedItems;
            _repositories.Bind(x => x.Filter, () => {

                Repositories.Refresh();
            });

        }

        public virtual Task Load(bool forceDataRefresh)
        {
            GitHubRequest<List<RepositoryModel>> request;
            if (string.Equals(Application.Account.Username, Username, StringComparison.OrdinalIgnoreCase))
                request = Application.Client.AuthenticatedUser.Repositories.GetAll();
            else
                request = Application.Client.Users[Username].Repositories.GetAll();
            return Repositories.SimpleCollectionLoad(request, forceDataRefresh);
        }

        private IEnumerable<IGrouping<string, RepositoryModel>> CreateGroupedItems(IEnumerable<RepositoryModel> model)
        {
            var order = (RepositoriesFilterModel.Order)Repositories.Filter.OrderBy;
            if (order == RepositoriesFilterModel.Order.Forks)
            {
                var a = model.OrderBy(x => x.Forks).GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.Forks));
                a = Repositories.Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return FilterGroup.CreateNumberedGroup(a, "Forks");
            }
            if (order == RepositoriesFilterModel.Order.LastUpdated)
            {
                var a = model.OrderByDescending(x => x.UpdatedAt).GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.UpdatedAt.TotalDaysAgo()));
                a = Repositories.Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return FilterGroup.CreateNumberedGroup(a, "Days Ago", "Updated");
            }
            if (order == RepositoriesFilterModel.Order.CreatedOn)
            {
                var a = model.OrderByDescending(x => x.CreatedAt).GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.CreatedAt.TotalDaysAgo()));
                a = Repositories.Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return FilterGroup.CreateNumberedGroup(a, "Days Ago", "Created");
            }
            if (order == RepositoriesFilterModel.Order.Followers)
            {
                var a = model.OrderBy(x => x.Watchers).GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.Watchers));
                a = Repositories.Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return FilterGroup.CreateNumberedGroup(a, "Followers");
            }
            if (order == RepositoriesFilterModel.Order.Owner)
            {
                var a = model.OrderBy(x => x.Name).GroupBy(x => x.Owner.Login);
                a = Repositories.Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return a.ToList();
            }

            return null;
        }
    }
}