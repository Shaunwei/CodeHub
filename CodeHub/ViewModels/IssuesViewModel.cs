using System;
using GitHubSharp.Models;
using System.Collections.Generic;
using MonoTouch.Dialog;
using System.Linq;
using CodeHub.Filters.Models;
using CodeFramework.ViewModels;
using System.Threading.Tasks;
using CodeHub.ViewModels;

namespace CodeHub.Controllers
{
    public class IssuesViewModel : FilterableCollectionViewModel<IssueModel, IssuesFilterModel>, ILoadableViewModel
    {
        public string User { get; private set; }
        public string Slug { get; private set; }

        public IssuesViewModel(string user, string slug)
            : base("IssuesViewModel")
        {
            User = user;
            Slug = slug;
        }

        public async Task Load(bool forceDataRefresh)
        {
            string direction = Filter.Ascending ? "asc" : "desc";
            string state = Filter.Open ? "open" : "closed";
            string sort = Filter.SortType == IssuesFilterModel.Sort.None ? null : Filter.SortType.ToString().ToLower();
            string labels = string.IsNullOrEmpty(Filter.Labels) ? null : Filter.Labels;
            string assignee = string.IsNullOrEmpty(Filter.Assignee) ? null : Filter.Assignee;
            string creator = string.IsNullOrEmpty(Filter.Creator) ? null : Filter.Creator;
            string mentioned = string.IsNullOrEmpty(Filter.Mentioned) ? null : Filter.Mentioned;

            var request = Application.Client.Users[User].Repositories[Slug].Issues.GetAll(sort: sort, labels: labels, state: state, direction: direction, 
                                                                                          assignee: assignee, creator: creator, mentioned: mentioned);

            await Task.Run(() => this.RequestModel(request, forceDataRefresh, response => {
                Items.Reset(response.Data);
                this.CreateMore(response, m => MoreItems = m, d => Items.AddRange(d));
            }));
        }

        protected override void FilterChanged()
        {
            throw new NotImplementedException();
        }

        protected List<IGrouping<string, IssueModel>> GroupModel(List<IssueModel> model, IssuesFilterModel filter)
        {
            var order = filter.SortType;
            if (order == IssuesFilterModel.Sort.Comments)
            {
                var a = filter.Ascending ? model.OrderBy(x => x.Comments) : model.OrderByDescending(x => x.Comments);
                var g = a.GroupBy(x => IntegerCeilings.First(r => r > x.Comments)).ToList();
                return CreateNumberedGroup(g, "Comments");
            }
            else if (order == IssuesFilterModel.Sort.Updated)
            {
                var a = filter.Ascending ? model.OrderBy(x => x.UpdatedAt) : model.OrderByDescending(x => x.UpdatedAt);
                var g = a.GroupBy(x => IntegerCeilings.First(r => r > x.UpdatedAt.TotalDaysAgo()));
                return CreateNumberedGroup(g, "Days Ago", "Updated");
            }
            else if (order == IssuesFilterModel.Sort.Created)
            {
                var a = filter.Ascending ? model.OrderBy(x => x.CreatedAt) : model.OrderByDescending(x => x.CreatedAt);
                var g = a.GroupBy(x => IntegerCeilings.First(r => r > x.CreatedAt.TotalDaysAgo()));
                return CreateNumberedGroup(g, "Days Ago", "Created");
            }

            return null;
        }

        public void CreateIssue(IssueModel issue)
        {
            if (!DoesIssueBelong(issue))
                return;
            Items.Add(issue);
        }

        public void UpdateIssue(IssueModel issue)
        {
            throw new NotImplementedException();
            Items.Remove(issue);
            if (DoesIssueBelong(issue))
                Items.Add(issue);
        }

        private bool DoesIssueBelong(IssueModel model)
        {
            if (Filter == null)
                return true;
            if (Filter.Open != model.State.Equals("open"))
                return false;
            return true;
        }
    }
}

