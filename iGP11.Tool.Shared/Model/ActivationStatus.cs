namespace iGP11.Tool.Shared.Model
{
    public enum ActivationStatus
    {
        NotRetrievable = 0,
        NotRunning,
        Running,
        PluginLoaded,
        PluginActivationPending,
        PluginActivated,
        PluginActivationFailed
    }
}