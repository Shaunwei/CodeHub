using CodeHub.ViewModels;

namespace CodeHub.ViewControllers
{
    public class UserEventsViewController : BaseEventsViewController
    {
        public UserEventsViewController(string username)
            : base(new UserEventsViewModel(username))
        {
            Title = "Events".t();
        }
    }
}