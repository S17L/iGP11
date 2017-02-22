using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace iGP11.Tool.Common
{
    public class ObservableRangeCollection<TEntry> : ObservableCollection<TEntry>
    {
        public ObservableRangeCollection()
        {
        }

        public ObservableRangeCollection(IEnumerable<TEntry> collection)
            : base(collection)
        {
        }

        public void AddRange(IEnumerable<TEntry> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var entry in collection)
            {
                Items.Add(entry);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void RemoveRange(IEnumerable<TEntry> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var entry in collection)
            {
                Items.Remove(entry);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Replace(TEntry entry)
        {
            ReplaceRange(new[] { entry });
        }

        public void ReplaceRange(IEnumerable<TEntry> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            Items.Clear();
            foreach (var entry in collection)
            {
                Items.Add(entry);
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}