using System;
using System.Threading.Tasks;

namespace iGP11.Library.Hub.Client.Action
{
    public class NotificationContext
    {
        internal bool Handled { get; private set; }

        internal DateTime? Usage { get; private set; }

        public void Complete()
        {
            Handled = true;
        }

        public async Task CompleteAsync()
        {
            Handled = true;
            await Task.Yield();
        }

        internal void KeepAlive()
        {
            Usage = DateTime.Now;
        }
    }
}