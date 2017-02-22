using iGP11.Tool.Model;

namespace iGP11.Tool.Events
{
    internal class ChangeViewEnabledEvent
    {
        public ChangeViewEnabledEvent(Target target, bool isEnabled)
        {
            Target = target;
            IsEnabled = isEnabled;
        }

        public bool IsEnabled { get; }

        public Target Target { get; }
    }
}