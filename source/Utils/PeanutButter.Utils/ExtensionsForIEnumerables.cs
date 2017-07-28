﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PeanutButter.Utils
{
    /// <summary>
    /// Useful extensions for IEnumerable&lt;T&gt; collections
    /// </summary>
    public static class ExtensionsForIEnumerables
    {
        /// <summary>
        /// The missing ForEach method
        /// </summary>
        /// <param name="collection">Subject collection to operate over</param>
        /// <param name="toRun">Action to run on each member of the collection</param>
        /// <typeparam name="T">Item type of the collection</typeparam>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> toRun)
        {
            foreach (var item in collection)
                toRun(item);
        }

        /// <summary>
        /// The missing ForEach method - async variant. Don't forget to await on it!
        /// </summary>
        /// <param name="collection">Subject collection to operate over</param>
        /// <param name="toRun">Action to run on each member of the collection</param>
        /// <typeparam name="T">Item type of the collection</typeparam>
//        public static async Task ForEachAsync<T>(this IEnumerable<T> collection, Func<T, Task> toRun)
//        {
//            foreach (var item in collection)
//                await toRun(item);
//        }

        /// <summary>
        /// The missing ForEach method - synchronous variant which also provides the current item index
        /// </summary>
        /// <param name="collection">Subject collection to operate over</param>
        /// <param name="toRunWithIndex">Action to run on each member of the collection</param>
        /// <typeparam name="T">Item type of the collection</typeparam>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T, int> toRunWithIndex)
        {
            var idx = 0;
            collection.ForEach(o => { toRunWithIndex(o, idx++); });
        }

        /// <summary>
        /// The missing ForEach method - asynchronous variant which also provides the current item index
        /// -> DON'T forget to await!
        /// </summary>
        /// <param name="collection">Subject collection to operate over</param>
        /// <param name="toRunWithIndex">Action to run on each member of the collection</param>
        /// <typeparam name="T">Item type of the collection</typeparam>
//        public static async Task ForEachAsync<T>(this IEnumerable<T> collection, Func<T, int, Task> toRunWithIndex)
//        {
//            var idx = 0;
//            await collection.ForEachAsync(async (o) => { await toRunWithIndex(o, idx++); });
//        }

        /// <summary>
        /// Calculates if two collections hold the same items, irrespective of order
        /// </summary>
        /// <param name="collection">Source collection</param>
        /// <param name="otherCollection">Collection to compare with</param>
        /// <typeparam name="T">Item tytpe of the collections</typeparam>
        /// <returns>True if all values in the source collection are found in the target collection</returns>
        public static bool IsSameAs<T>(this IEnumerable<T> collection, IEnumerable<T> otherCollection)
        {
            if (collection == null && otherCollection == null) return true;
            if (collection == null || otherCollection == null) return false;
            var source = collection.ToArray();
            var target = otherCollection.ToArray();
            if (source.Count() != target.Count()) return false;
            return source.Aggregate(true, (state, item) => state && target.Contains(item));
        }

        /// <summary>
        /// Fluent alternative to string.Join()
        /// </summary>
        /// <param name="collection">Source collection to operate on</param>
        /// <param name="joinWith">String to join items with</param>
        /// <typeparam name="T">Underlying type of the collection</typeparam>
        /// <returns>
        /// string representing items of the collection joined with the joinWith parameter.
        /// Where a collection of non-strings is provided, the objects' ToString() methods
        /// are used to get a string representation.
        /// </returns>
        public static string JoinWith<T>(this IEnumerable<T> collection, string joinWith)
        {
            var stringArray = collection as string[];
            if (stringArray == null)
            {
                if (typeof(T) == typeof(string))
                    stringArray = collection.ToArray() as string[];
                else
                    stringArray = collection.Select(i => i.ToString()).ToArray();
            }
            return string.Join(joinWith, stringArray ?? new string[0]);
        }

        /// <summary>
        /// Convenience method, essentially opposite to Any(), except
        /// that it also handles null collections
        /// </summary>
        /// <param name="collection">Source collection to operate on</param>
        /// <typeparam name="T">Item type of the collection</typeparam>
        /// <returns>True if the collection is null or has no items; false otherwise.</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection == null) return true;
            return !collection.Any();
        }

        /// <summary>
        /// Convenience method to mitigate null checking and errors when
        /// a null collection can be treated as if it were empty, eg:
        /// someCollection.EmptyIfNull().ForEach(DoSomething);
        /// </summary>
        /// <param name="collection">Source collection to operate over</param>
        /// <typeparam name="T">Item type of the collection</typeparam>
        /// <returns>An empty collection if the source is null; otherwise the source.</returns>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
        {
            return collection ?? new T[0];
        }

        /// <summary>
        /// Convenience method to create a new array with the provided element(s) appended
        /// </summary>
        /// <param name="source">Source array to start with</param>
        /// <param name="toAdd">Item(s) to add to the result array</param>
        /// <typeparam name="T">Item type of the array</typeparam>
        /// <returns>A new array which is the source with the new items appended</returns>
        public static T[] And<T>(this IEnumerable<T> source, params T[] toAdd)
        {
            return source.Concat(toAdd).ToArray();
        }

        /// <summary>
        /// Convenience / fluent method to provide an array without the provided item(s)
        /// </summary>
        /// <param name="source">Source collection</param>
        /// <param name="toRemove">items which should not appear in the result array</param>
        /// <typeparam name="T">Item type of the array</typeparam>
        /// <returns>A new array of T with the specified items not present</returns>
        public static T[] ButNot<T>(this IEnumerable<T> source, params T[] toRemove)
        {
            return source.Except(toRemove).ToArray();
        }

        /// <summary>
        /// Convenience wrapper around SelectMany; essentially flattens a nested collection 
        /// of collection(s) of some item. Exactly equivalent to:
        /// collection.SelectMany(o => o);
        /// </summary>
        /// <param name="collection">Source collection to operate on</param>
        /// <typeparam name="T">Item type of the collection</typeparam>
        /// <returns>A new, flat collection</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> collection)
        {
            return collection.SelectMany(o => o);
        }

        /// <summary>
        /// Convenience method to get the results of a selection where the results are non-null
        /// -> this variant works on Nullable types
        /// </summary>
        /// <param name="collection">Source collection to operate over</param>
        /// <param name="grabber">Function to grab the data you're interested in off of each source item</param>
        /// <typeparam name="TCollection">Item type of the source collection</typeparam>
        /// <typeparam name="TResult">Item type of the result collection</typeparam>
        /// <returns>
        /// A new collection which is the result of a Select with the provided grabber
        /// where the Select results are non-null
        /// </returns>
        public static IEnumerable<TResult> SelectNonNull<TCollection, TResult>(
            this IEnumerable<TCollection> collection,
            Func<TCollection, TResult?> grabber) where TResult : struct
        {
            return collection
                .Select(grabber)
                .Where(i => i.HasValue)
                .Select(i => i.Value);
        }

        /// <summary>
        /// Convenience method to get the results of a selection where the results are non-null
        /// -> this variant works on types which can natively hold the value null
        /// </summary>
        /// <param name="collection">Source collection to operate over</param>
        /// <param name="grabber">Function to grab the data you're interested in off of each source item</param>
        /// <typeparam name="TCollection">Item type of the source collection</typeparam>
        /// <typeparam name="TResult">Item type of the result collection</typeparam>
        /// <returns>
        /// A new collection which is the result of a Select with the provided grabber
        /// where the Select results are non-null
        /// </returns>
        public static IEnumerable<TResult> SelectNonNull<TCollection, TResult>(
            this IEnumerable<TCollection> collection,
            Func<TCollection, TResult> grabber) where TResult : class
        {
            return collection
                .Select(grabber)
                .Where(i => i != null)
                .Select(i => i);
        }

        /// <summary>
        /// Convenience method to produce a block of text from a collection of items
        /// -> optionally, delimit with a string of your choice instead of a newline
        /// -> essentially a wrapper around JoinWith()
        /// </summary>
        /// <param name="input">Source input lines</param>
        /// <param name="delimiter">Optional delimiter (default is Environment.NewLine)</param>
        /// <typeparam name="T">Item type of collection</typeparam>
        /// <returns>String representation of the the items</returns>
        public static string AsText<T>(this IEnumerable<T> input, string delimiter = null)
        {
            return input.JoinWith(delimiter ?? Environment.NewLine);
        }

        /// <summary>
        /// Convenience method to test if a collection has a single item matching the 
        /// provided matcher function
        /// </summary>
        /// <param name="input">Source collection</param>
        /// <param name="matcher">Function to run over each item to test if it passes</param>
        /// <typeparam name="T">Item type of the collection</typeparam>
        /// <returns>
        /// True if only one item in the collection got a true value from the matcher
        /// function; false if zero or more than one items were matched.
        /// </returns>
        public static bool HasUnique<T>(this IEnumerable<T> input, Func<T, bool> matcher)
        {
            var matches = input.Where(matcher);
            return matches.Count() == 1;
        }

        /// <summary>
        /// Fluency method to run an action a certain number of times, eg:
        /// 10.TimesDo(() => Console.WriteLine("Hello World"));
        /// </summary>
        /// <param name="howMany">Number of times to run the provided action</param>
        /// <param name="toRun">Action to run</param>
        public static void TimesDo(this int howMany, Action toRun)
        {
            howMany.TimesDo(i => toRun());
        }

        /// <summary>
        /// Fluency method to run an action a certain number of times. This
        /// variant runs on an action given the current index at each run, eg:
        /// 10.TimesDo(i => Console.WriteLine($"This action has run {i} times"));
        /// </summary>
        /// <param name="howMany">Number of times to run the provided action</param>
        /// <param name="toRun">Action to run</param>
        public static void TimesDo(this int howMany, Action<int> toRun)
        {
            if (howMany < 0)
                throw new ArgumentException("TimesDo must be called on positive integer", nameof(howMany));
            for (var i = 0; i < howMany; i++)
                toRun(i);
        }

        /// <summary>
        /// Convenience method to get the second item from a collection
        /// </summary>
        /// <param name="src">Source collection</param>
        /// <typeparam name="T">Item type of the collection</typeparam>
        /// <returns>The second item, when available. Will throw if there is no item available.</returns>
        public static T Second<T>(this IEnumerable<T> src)
        {
            return src.FirstAfter(1);
        }

        /// <summary>
        /// Convenience method to get the third item from a collection
        /// </summary>
        /// <param name="src">Source collection</param>
        /// <typeparam name="T">Item type of the collection</typeparam>
        /// <returns>The third item, when available. Will throw if there is no item available.</returns>
        public static T Third<T>(this IEnumerable<T> src)
        {
            return src.FirstAfter(2);
        }

        /// <summary>
        /// Convenience method to get the first item after skipping N items from a collection
        /// -> equivalent to collection.Skip(N).First();
        /// -> collection.FirtstAfter(2) returns the 3rd element
        /// </summary>
        /// <param name="src">Source collection</param>
        /// <param name="toSkip">How many items to skip</param>
        /// <typeparam name="T">Item type of the collection</typeparam>
        /// <returns>The third item, when available. Will throw if there is no item available.</returns>
        public static T FirstAfter<T>(this IEnumerable<T> src, int toSkip)
        {
            return src.Skip(toSkip).First();
        }

        /// <summary>
        /// Convenience method to get the first item after skipping N items from a collection
        /// -> equivalent to collection.Skip(N).First();
        /// -> collection.FirtstAfter(2) returns the 3rd element
        /// -> this variant returns the default value for T if the N is out of bounds
        /// </summary>
        /// <param name="src">Source collection</param>
        /// <param name="toSkip">How many items to skip</param>
        /// <typeparam name="T">Item type of the collection</typeparam>
        /// <returns>The third item, when available. Will return the default value for T otherwise.</returns>
        public static T FirstOrDefaultAfter<T>(this IEnumerable<T> src, int toSkip)
        {
            return src.Skip(toSkip).FirstOrDefault();
        }
    }
}