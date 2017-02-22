using System;

namespace iGP11.Library
{
    public sealed class ProcessingScope : IDisposable
    {
        private readonly bool _oldValue;
        private readonly IProcessable _processable;

        public ProcessingScope(IProcessable processable)
        {
            _processable = processable;
            _oldValue = _processable.IsProcessing;
            _processable.IsProcessing = true;
        }

        public void Dispose()
        {
            _processable.IsProcessing = _oldValue;
        }
    }
}