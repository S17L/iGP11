using iGP11.Tool.Model;

namespace iGP11.Tool.Events
{
    public class UpdateStatusEvent
    {
        public UpdateStatusEvent(Target target, StatusType type, string text)
        {
            Target = target;
            Type = type;
            Text = text;
        }

        public Target Target { get; }

        public string Text { get; }

        public StatusType Type { get; }
    }
}