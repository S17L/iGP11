using System;
using System.Collections.Generic;
using System.Linq;

namespace iGP11.Library
{
    public static class EnumerableExtensions
    {
        private static readonly Random _random = new Random(Guid.NewGuid().GetHashCode());

        public static TEntry AddAndReturn<TEntry>(this ICollection<TEntry> collection, TEntry entry)
        {
            collection.Add(entry);

            return entry;
        }

        public static IEnumerable<TEntry> Distinct<TEntry, TProperty>(this IEnumerable<TEntry> collection, Func<TEntry, TProperty> selector)
        {
            return collection.GroupBy(selector)
                .Select(group => group.First())
                .ToArray();
        }

        public static IEnumerable<TEntry> ExceptFirst<TEntry>(this IEnumerable<TEntry> collection)
        {
            return collection.Where((entry, i) => i > 0).ToArray();
        }

        public static IEnumerable<TEntry> ExceptLast<TEntry>(this IEnumerable<TEntry> collection)
        {
            var array = collection.ToCollection();
            return array.Where((entry, i) => i < array.Length - 1).ToArray();
        }

        public static void ForEach<TEntry>(this IEnumerable<TEntry> collection, Action<TEntry> action)
        {
            foreach (var entry in collection)
            {
                action(entry);
            }
        }

        public static void ForEach<TEntry>(this IEnumerable<TEntry> collection, Action<TEntry, int> action)
        {
            var array = collection.ToCollection();
            for (var i = 0; i < array.Length; i++)
            {
                action(array[i], i);
            }
        }

        public static int IndexOf<TEntry>(this IEnumerable<TEntry> collection, Predicate<TEntry> filter)
        {
            var array = collection.ToCollection();
            for (var i = 0; i < array.Length; i++)
            {
                if (filter(array[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool IsNotNullOrEmpty<TEntry>(this IEnumerable<TEntry> collection)
        {
            return !collection.IsNullOrEmpty();
        }

        public static bool IsNullOrEmpty<TEntry>(this IEnumerable<TEntry> collection)
        {
            return (collection == null) || !collection.Any();
        }

        public static void Remove<TEntry>(this ICollection<TEntry> collection, Predicate<TEntry> filter)
        {
            if (collection.IsNullOrEmpty())
            {
                return;
            }

            foreach (var entry in collection.ToArray())
            {
                if (filter(entry))
                {
                    collection.Remove(entry);
                }
            }
        }

        public static IEnumerable<TObject> RemoveEmpty<TObject>(this IEnumerable<TObject> collection) where TObject : class
        {
            return collection.ToCollection()
                .Where(entry => entry != null)
                .ToArray();
        }

        public static IEnumerable<string> RemoveEmpty(this IEnumerable<string> collection)
        {
            return collection.ToCollection()
                .Where(entry => !entry.IsNullOrEmpty())
                .ToArray();
        }

        public static void Shuffle<TEntry>(this IList<TEntry> list)
        {
            var count = list.Count;
            while (count > 1)
            {
                count--;
                var index = _random.Next(count + 1);
                var value = list[index];
                list[index] = list[count];
                list[count] = value;
            }
        }

        public static TEntry[] ToCollection<TEntry>(this IEnumerable<TEntry> collection)
        {
            if (collection == null)
            {
                return new TEntry[0];
            }

            return collection as TEntry[] ?? collection.ToArray();
        }
    }
}