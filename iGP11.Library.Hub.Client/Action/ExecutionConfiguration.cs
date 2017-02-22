using System;

namespace iGP11.Library.Hub.Client.Action
{
    public class ExecutionConfiguration
    {
        public TimeSpan ActionTimeout { get; private set; } = TimeSpan.FromSeconds(15);

        public TimeSpan EventTimeout { get; private set; } = TimeSpan.FromSeconds(15);

        public TimeSpan Tick { get; private set; } = TimeSpan.FromMilliseconds(100);

        public ExecutionConfiguration SetActionTimeout(TimeSpan timeSpan)
        {
            ActionTimeout = timeSpan;
            return this;
        }

        public ExecutionConfiguration SetEventTimeout(TimeSpan timeSpan)
        {
            ActionTimeout = timeSpan;
            return this;
        }

        public ExecutionConfiguration SetTick(TimeSpan timeSpan)
        {
            Tick = timeSpan;
            return this;
        }
    }
}