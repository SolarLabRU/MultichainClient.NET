using System;

namespace Platform.DataAccess.MultiChain.Client
{
    public class EventArgs<T> : EventArgs
    {
        public T Item { get; set; }

        public EventArgs()
        {
        }

        public EventArgs(T item)
        {
            this.Item = item;
        }
    }
}
