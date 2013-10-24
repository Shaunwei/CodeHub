using System;
using GitHubSharp.Models;
using CodeHub.Controllers;
using MonoTouch.Dialog;
using CodeFramework.ViewControllers;
using CodeHub.ViewModels;
using System.Linq;

namespace CodeHub.Filters.ViewControllers
{
    public class IssueMilestonesFilterViewController : ViewModelCollectionDrivenViewController
    {
        public Action<string, uint?, string> MilestoneSelected;

        public new IssueMilestonesViewModel ViewModel
        {
            get { return (IssueMilestonesViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        public IssueMilestonesFilterViewController(string user, string repo, bool alreadySelected)
        {
            Title = "Milestones".t();
            NoItemsText = "No Milestones".t();
            SearchPlaceholder = "Search Milestones".t();
            ViewModel = new IssueMilestonesViewModel(user, repo);

            var clearMilestone = new MilestoneModel { Title = "Clear milestone filter".t() };
            var noMilestone = new MilestoneModel { Title = "Issues with no milestone".t() };
            var withMilestone = new MilestoneModel { Title = "Issues with milestone".t() };

            //Add a fake 'Unassigned' guy so we can always unassigned what we've done
            ViewModel.BindCollection(x => x.Milestones, (ev) =>
            {
                var items = ViewModel.Milestones.ToList();

                items.Insert(0, noMilestone);
                items.Insert(1, withMilestone);

                if (alreadySelected)
                    items.Insert(0, clearMilestone);

                RenderList(items, x => {
                    return new StyledStringElement(x.Title, () => {
                        if (MilestoneSelected != null)
                        {
                            if (x == noMilestone)
                                MilestoneSelected(x.Title, null, "none");
                            else if (x == withMilestone)
                                MilestoneSelected(x.Title, null, "*");
                            else if (x == clearMilestone)
                                MilestoneSelected(null, null, null);
                            else
                                MilestoneSelected(x.Title, x.Number, x.Number.ToString());
                        }
                    });
                }, ViewModel.Milestones.MoreItems);
            });
        }
    }
}

