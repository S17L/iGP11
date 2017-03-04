namespace iGP11.Tool.Application
{
    public interface IWatchableProcess
    {
        string FilePath { get; }

        void OnStarted();

        void OnTerminated();
    }
}