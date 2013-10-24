using System;
using GitHubSharp.Models;
using CodeHub.Controllers;
using MonoTouch.Dialog;
using CodeFramework.ViewControllers;
using CodeHub.ViewModels;
using System.Linq;

namespace CodeHub.ViewControllers
{
    public class IssueMilestonesViewController : ViewModelCollectionDrivenViewController
    {
        public Action<MilestoneModel> MilestoneSelected;

        public new IssueMilestonesViewModel ViewModel
        {
            get { return (IssueMilestonesViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        public IssueMilestonesViewController(string user, string repo)
        {
            Title = "Milestones".t();
            NoItemsText = "No Milestones".t();
            SearchPlaceholder = "Search Milestones".t();
            ViewModel = new IssueMilestonesViewModel(user, repo);

            //Add a fake 'Unassigned' guy so we can always unassigned what we've done
            ViewModel.BindCollection(x => x.Milestones, (ev) =>
            {
                var items = ViewModel.Milestones.ToList();
                var noMilestone = new MilestoneModel { Title = "No Milestone".t() };
                items.Insert(0, noMilestone);

                RenderList(items, x => {
                    return new StyledStringElement(x.Title, () => {
                        if (MilestoneSelected != null)
                            MilestoneSelected(x == noMilestone ? null : x);
                    });
                }, ViewModel.Milestones.MoreItems);
            });
        }
    }
}

