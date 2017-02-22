using iGP11.Tool.Model;

namespace iGP11.Tool.Events
{
    public class ApplicationActionEvent
    {
        public ApplicationActionEvent(ApplicationAction action)
        {
            Action = action;
        }

        public ApplicationAction Action { get; }
    }
}