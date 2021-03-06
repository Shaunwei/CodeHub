using System;
using CodeFramework.ViewModels;
using GitHubSharp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CodeFramework.Utils;

namespace CodeHub.ViewModels
{
    public class BranchesViewModel : ViewModel, ILoadableViewModel
    {
        private readonly CollectionViewModel<BranchModel> _items = new CollectionViewModel<BranchModel>();

        public string Username
        {
            get;
            private set;
        }

        public string Repository
        {
            get;
            private set;
        }

        public CollectionViewModel<BranchModel> Branches
        {
            get { return _items; }
        }

        public BranchesViewModel(string username, string repository)
        {
            Username = username;
            Repository = repository;
        }

        public Task Load(bool forceDataRefresh)
        {
            return Branches.SimpleCollectionLoad(Application.Client.Users[Username].Repositories[Repository].GetBranches(), forceDataRefresh);
        }
    }
}

