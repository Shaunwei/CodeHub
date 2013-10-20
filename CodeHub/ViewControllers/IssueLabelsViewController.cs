using System;
using System.Linq;
using GitHubSharp.Models;
using CodeHub.Controllers;
using MonoTouch.Dialog;
using CodeFramework.Elements;
using System.Collections.Generic;
using CodeFramework.ViewControllers;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class IssueLabelsViewController : ViewModelCollectionDrivenViewController
    {
        public List<LabelModel> SelectedLabels { get; set; }

        public new RepositoryLabelsViewModel ViewModel
        {
            get { return (RepositoryLabelsViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public IssueLabelsViewController(string user, string repo)
        {
            Title = "Labels".t();
            NoItemsText = "No Labels".t();
            SearchPlaceholder = "Search Labels".t();
            ViewModel = new RepositoryLabelsViewModel(user, repo);
            SelectedLabels = new List<LabelModel>();

            BindCollection(ViewModel, x => {
                var e = new StyledStringElement(x.Name);

                if (SelectedLabels.Exists(y => y.Name.Equals(x.Name)))
                    e.Accessory = MonoTouch.UIKit.UITableViewCellAccessory.Checkmark;

                e.Tapped += () => {
                    if (e.Accessory == MonoTouch.UIKit.UITableViewCellAccessory.Checkmark)
                    {
                        SelectedLabels.RemoveAll(y => y.Name.Equals(x.Name));
                        e.Accessory = MonoTouch.UIKit.UITableViewCellAccessory.None;
                    }
                    else
                    {
                        SelectedLabels.Add(x);
                        e.Accessory = MonoTouch.UIKit.UITableViewCellAccessory.Checkmark;
                    }

                    Root.Reload(e, MonoTouch.UIKit.UITableViewRowAnimation.None);
                };

                return e;
            });
        }
    }
}

