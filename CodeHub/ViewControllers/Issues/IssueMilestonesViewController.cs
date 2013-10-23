using System;
using GitHubSharp.Models;
using CodeHub.Controllers;
using MonoTouch.Dialog;
using CodeFramework.ViewControllers;
using CodeHub.ViewModels;

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

            BindCollection(ViewModel, x => {
                return new StyledStringElement(x.Title, () => {
                    if (MilestoneSelected != null)
                        MilestoneSelected(x);
                });
            });
        }
    }
}

