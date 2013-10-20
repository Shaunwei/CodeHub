using System;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class OrganizationRepositoriesViewModel : RepositoriesViewModel
    {
        public string Name
        {
            get;
            private set;
        }

        public OrganizationRepositoriesViewModel(string name)
            : base(name)
        {
            Name = name;
        }

        public override async Task Load(bool forceDataRefresh)
        {
            await this.SimpleCollectionLoad(Application.Client.Organizations[Name].Repositories.GetAll(), forceDataRefresh);
        }
    }
}

