using System.Collections.Generic;

using iGP11.Tool.Shared.Model;

namespace iGP11.Tool.ReadModel.Api
{
    public class PluginStateEqualityComparer : IEqualityComparer<ProxySettings>
    {
        private readonly bool _isCheckingActivationStatus;

        public PluginStateEqualityComparer(bool isCheckingActivationStatus = false)
        {
            _isCheckingActivationStatus = isCheckingActivationStatus;
        }

        public bool Equals(ProxySettings first, ProxySettings second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if ((first == null) || (second == null))
            {
                return false;
            }

            return (first.GameFilePath == second.GameFilePath) && (first.ProxyDirectoryPath == second.ProxyDirectoryPath) && (first.PluginType == second.PluginType) && (!_isCheckingActivationStatus || (first.ActivationStatus != second.ActivationStatus));
        }

        public int GetHashCode(ProxySettings state)
        {
            return state.GetHashCode();
        }
    }
}