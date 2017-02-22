namespace iGP11.Tool.Events
{
    public class ShowErrorEvent
    {
        public ShowErrorEvent(string error)
        {
            Error = error;
        }

        public string Error { get; }
    }
}