using System;
using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class OrganizationEventsViewController : BaseEventsViewController
    {
        public OrganizationEventsViewController(string userName, string orgName)
            : base(new OrganizationEventsViewModel(userName, orgName))
        {
        }
    }
}

