#region License and Terms

// MoreLINQ - Extensions to LINQ to Objects
// Copyright (c) 2008 Jonathan Skeet. All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

namespace EnsoulSharp.SDK.MoreLinq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    static partial class MoreEnumerable
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Groups the adjacent elements of a sequence according to a
        ///     specified key selector function.
        /// </summary>
        /// <typeparam name="TSource">
        ///     The type of the elements of
        ///     <paramref name="source" />.
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     The type of the key returned by
        ///     <paramref name="keySelector" />.
        /// </typeparam>
        /// <param name="source">A sequence whose elements to group.</param>
        /// <param name="keySelector">
        ///     A function to extract the key for each
        ///     element.
        /// </param>
        /// <returns>
        ///     A sequence of groupings where each grouping
        ///     (<see cref="IGrouping{TKey,TElement}" />) contains the key
        ///     and the adjacent elements in the same order as found in the
        ///     source sequence.
        /// </returns>
        /// <remarks>
        ///     This method is implemented by using deferred execution and
        ///     streams the groupings. The grouping elements, however, are
        ///     buffered. Each grouping is therefore yielded as soon as it
        ///     is complete and before the next grouping occurs.
        /// </remarks>
        public static IEnumerable<IGrouping<TKey, TSource>> GroupAdjacent<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return GroupAdjacent(source, keySelector, null);
        }

        /// <summary>
        ///     Groups the adjacent elements of a sequence according to a
        ///     specified key selector function and compares the keys by using a
        ///     specified comparer.
        /// </summary>
        /// <typeparam name="TSource">
        ///     The type of the elements of
        ///     <paramref name="source" />.
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     The type of the key returned by
        ///     <paramref name="keySelector" />.
        /// </typeparam>
        /// <param name="source">A sequence whose elements to group.</param>
        /// <param name="keySelector">
        ///     A function to extract the key for each
        ///     element.
        /// </param>
        /// <param name="comparer">
        ///     An <see cref="IEqualityComparer{T}" /> to
        ///     compare keys.
        /// </param>
        /// <returns>
        ///     A sequence of groupings where each grouping
        ///     (<see cref="IGrouping{TKey,TElement}" />) contains the key
        ///     and the adjacent elements in the same order as found in the
        ///     source sequence.
        /// </returns>
        /// <remarks>
        ///     This method is implemented by using deferred execution and
        ///     streams the groupings. The grouping elements, however, are
        ///     buffered. Each grouping is therefore yielded as soon as it
        ///     is complete and before the next grouping occurs.
        /// </remarks>
        public static IEnumerable<IGrouping<TKey, TSource>> GroupAdjacent<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            return GroupAdjacent(source, keySelector, e => e, comparer);
        }

        /// <summary>
        ///     Groups the adjacent elements of a sequence according to a
        ///     specified key selector function and projects the elements for
        ///     each group by using a specified function.
        /// </summary>
        /// <typeparam name="TSource">
        ///     The type of the elements of
        ///     <paramref name="source" />.
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     The type of the key returned by
        ///     <paramref name="keySelector" />.
        /// </typeparam>
        /// <typeparam name="TElement">
        ///     The type of the elements in the
        ///     resulting groupings.
        /// </typeparam>
        /// <param name="source">A sequence whose elements to group.</param>
        /// <param name="keySelector">
        ///     A function to extract the key for each
        ///     element.
        /// </param>
        /// <param name="elementSelector">
        ///     A function to map each source
        ///     element to an element in the resulting grouping.
        /// </param>
        /// <returns>
        ///     A sequence of groupings where each grouping
        ///     (<see cref="IGrouping{TKey,TElement}" />) contains the key
        ///     and the adjacent elements (of type <typeparamref name="TElement" />)
        ///     in the same order as found in the source sequence.
        /// </returns>
        /// <remarks>
        ///     This method is implemented by using deferred execution and
        ///     streams the groupings. The grouping elements, however, are
        ///     buffered. Each grouping is therefore yielded as soon as it
        ///     is complete and before the next grouping occurs.
        /// </remarks>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupAdjacent<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            return GroupAdjacent(source, keySelector, elementSelector, null);
        }

        /// <summary>
        ///     Groups the adjacent elements of a sequence according to a
        ///     specified key selector function. The keys are compared by using
        ///     a comparer and each group's elements are projected by using a
        ///     specified function.
        /// </summary>
        /// <typeparam name="TSource">
        ///     The type of the elements of
        ///     <paramref name="source" />.
        /// </typeparam>
        /// <typeparam name="TKey">
        ///     The type of the key returned by
        ///     <paramref name="keySelector" />.
        /// </typeparam>
        /// <typeparam name="TElement">
        ///     The type of the elements in the
        ///     resulting groupings.
        /// </typeparam>
        /// <param name="source">A sequence whose elements to group.</param>
        /// <param name="keySelector">
        ///     A function to extract the key for each
        ///     element.
        /// </param>
        /// <param name="elementSelector">
        ///     A function to map each source
        ///     element to an element in the resulting grouping.
        /// </param>
        /// <param name="comparer">
        ///     An <see cref="IEqualityComparer{T}" /> to
        ///     compare keys.
        /// </param>
        /// <returns>
        ///     A sequence of groupings where each grouping
        ///     (<see cref="IGrouping{TKey,TElement}" />) contains the key
        ///     and the adjacent elements (of type <typeparamref name="TElement" />)
        ///     in the same order as found in the source sequence.
        /// </returns>
        /// <remarks>
        ///     This method is implemented by using deferred execution and
        ///     streams the groupings. The grouping elements, however, are
        ///     buffered. Each grouping is therefore yielded as soon as it
        ///     is complete and before the next grouping occurs.
        /// </remarks>
        public static IEnumerable<IGrouping<TKey, TElement>> GroupAdjacent<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            if (elementSelector == null)
            {
                throw new ArgumentNullException("elementSelector");
            }

            return GroupAdjacentImpl(source, keySelector, elementSelector, comparer ?? EqualityComparer<TKey>.Default);
        }

        #endregion

        #region Methods

        private static Grouping<TKey, TElement> CreateGroupAdjacentGrouping<TKey, TElement>(
            TKey key,
            IList<TElement> members)
        {
            Debug.Assert(members != null);
            return Grouping.Create(key, members.IsReadOnly ? members : new ReadOnlyCollection<TElement>(members));
        }

        private static IEnumerable<IGrouping<TKey, TElement>> GroupAdjacentImpl<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            Debug.Assert(source != null);
            Debug.Assert(keySelector != null);
            Debug.Assert(elementSelector != null);
            Debug.Assert(comparer != null);

            using (
                var iterator =
                    source.Select(item => new KeyValuePair<TKey, TElement>(keySelector(item), elementSelector(item)))
                        .GetEnumerator())
            {
                var group = default(TKey);
                var members = (List<TElement>)null;

                while (iterator.MoveNext())
                {
                    var item = iterator.Current;
                    if (members != null && comparer.Equals(group, item.Key))
                    {
                        members.Add(item.Value);
                    }
                    else
                    {
                        if (members != null)
                        {
                            yield return CreateGroupAdjacentGrouping(group, members);
                        }
                        group = item.Key;
                        members = new List<TElement> { item.Value };
                    }
                }

                if (members != null)
                {
                    yield return CreateGroupAdjacentGrouping(group, members);
                }
            }
        }

        #endregion

        static class Grouping
        {
            #region Public Methods and Operators

            public static Grouping<TKey, TElement> Create<TKey, TElement>(TKey key, IEnumerable<TElement> members)
            {
                return new Grouping<TKey, TElement>(key, members);
            }

            #endregion
        }

#if !NO_SERIALIZATION_ATTRIBUTES
        [Serializable]
#endif
        private sealed class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
        {
            #region Fields

            private readonly IEnumerable<TElement> _members;

            #endregion

            #region Constructors and Destructors

            public Grouping(TKey key, IEnumerable<TElement> members)
            {
                Debug.Assert(members != null);
                this.Key = key;
                this._members = members;
            }

            #endregion

            #region Public Properties

            public TKey Key { get; private set; }

            #endregion

            #region Public Methods and Operators

            public IEnumerator<TElement> GetEnumerator()
            {
                return this._members.GetEnumerator();
            }

            #endregion

            #region Explicit Interface Methods

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            #endregion
        }
    }
}