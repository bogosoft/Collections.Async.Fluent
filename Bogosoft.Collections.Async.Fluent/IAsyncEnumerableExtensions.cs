using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent
{
    /// <summary>
    /// Extended functionality for the <see cref="IAsyncEnumerable{T}"/> contract.
    /// </summary>
    public static class IAsyncEnumerableExtensions
    {
        /// <summary>
        /// Determine whether all elements in a sequence satsify a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A predicate to apply to each element in the sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// True if every element in the current sequence satisfies the given conidition; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the source sequence of predicate is null.
        /// </exception>
        public static async Task<bool> AllAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, bool> predicate,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            using (var enumerator = source.GetEnumerator())
            {
                while (await enumerator.MoveNextAsync(token))
                {
                    if (!predicate.Invoke(enumerator.Current))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Determine whether the current sequence contains any elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// True if the current sequence contains at least one element; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static async Task<bool> AnyAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var enumerator = source.GetEnumerator())
            {
                return await enumerator.MoveNextAsync(token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Determine if at least one element in the current sequence satisfies a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A predicate to be applied to all elements in the current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// True if at least one element in the current sequence satisfies the given condition; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the given source or predicate is null.
        /// </exception>
        public static Task<bool> AnyAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, bool> predicate,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return source.Where(predicate).AnyAsync(token);
        }

        /// <summary>
        /// Configure the current sequence so that, when enumerated, a given
        /// action is invoked on each enumerated element.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="action">
        /// An action to invoke on each element of the current sequence.
        /// </param>
        /// <returns>
        /// The current sequence configured to invoke an action on each element of the
        /// current sequence once enumeration has started.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current sequence or the given action are null.
        /// </exception>
        public static IAsyncEnumerable<T> Apply<T>(this IAsyncEnumerable<T> source, Action<T> action)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            return new DelegateAppliedSequence<T>(source, action);
        }

        /// <summary>
        /// Enumerate the current sequence, applying a given action to each enumerated element.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="action">An action to apply to each element in the current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>A task representing an asynchronous operation.</returns>
        public static async Task ApplyAsync<T>(
            this IAsyncEnumerable<T> source,
            Action<T> action,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            using (var enumerator = source.GetEnumerator())
            {
                while (await enumerator.MoveNextAsync(token).ConfigureAwait(false))
                {
                    action.Invoke(enumerator.Current);
                }
            }
        }

        /// <summary>
        /// Enumerate the current sequence, applying a given action to each enumerated element.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="action">An action to apply to each element in the current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>A task representing an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence or given action is null.
        /// </exception>
        public static async Task ApplyAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, CancellationToken, Task> action,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            using (var enumerator = source.GetEnumerator())
            {
                while (await enumerator.MoveNextAsync(token).ConfigureAwait(false))
                {
                    await action.Invoke(enumerator.Current, token).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Combine the current sequence with another.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="target">A sequence of elements append to the current sequence.</param>
        /// <returns>
        /// A single, concatenated sequence consisting of the current sequence and another sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current or given sequence is null.
        /// </exception>
        public static IAsyncEnumerable<T> Concat<T>(this IAsyncEnumerable<T> source, IAsyncEnumerable<T> target)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            return new ConcatenatedSequence<T>(source, target);
        }

        /// <summary>
        /// Determine whether the current sequence contains a given item
        /// using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="item">An item to look for in the current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// True if the current sequence contains the given item; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static Task<bool> ContainsAsync<T>(
            this IAsyncEnumerable<T> source,
            T item,
            CancellationToken token = default(CancellationToken)
            )
        {
            return source.ContainsAsync(item, EqualityComparer<T>.Default, token);
        }

        /// <summary>
        /// Determine whether the current sequence contains a given item using a given equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="item">An item to look for in the current sequence.</param>
        /// <param name="comparer">A comparison strategy to use when looking for the given item.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// True if the current sequence contains the given item; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence or given comparer is null.
        /// </exception>
        public static async Task<bool> ContainsAsync<T>(
            this IAsyncEnumerable<T> source,
            T item,
            IEqualityComparer<T> comparer,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            using (var enumerator = source.GetEnumerator())
            {
                while (await enumerator.MoveNextAsync(token).ConfigureAwait(false))
                {
                    if (comparer.Equals(item, enumerator.Current))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Count the number of elements in the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// A value corresponding to the number of elements in the current sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static async Task<ulong> CountAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var enumerator = source.GetEnumerator())
            {
                ulong total = 0;

                while (await enumerator.MoveNextAsync(token).ConfigureAwait(false))
                {
                    ++total;
                }

                return total;
            }
        }

        /// <summary>
        /// Count the number of elements in the current sequence that satisfy a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A condition to check each element of the current sequence against.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// A value corresponding to the number of elements in the current sequence
        /// that satisfy the given condition.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence or given predicate is null.
        /// </exception>
        public static Task<ulong> CountAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, bool> predicate,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return source.Where(predicate).CountAsync(token);
        }

        /// <summary>
        /// Condense the current sequence into a sequence of only its distinct elements. Elements will be
        /// compared to one another using the <see cref="EqualityComparer{T}.Default"/> value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <returns>A sequence of distinct elements.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static IAsyncEnumerable<T> Distinct<T>(this IAsyncEnumerable<T> source)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new DistinctSequence<T>(source, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Condense the current sequence into a sequence of only its distinct elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="comparer">A strategy for comparing the equality of elements.</param>
        /// <returns>A sequence of distinct elements.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current sequence or the given comparer is null.
        /// </exception>
        public static IAsyncEnumerable<T> Distinct<T>(this IAsyncEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            return new DistinctSequence<T>(source, comparer);
        }

        /// <summary>
        /// Get the first element in the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>The first element in the current sequence if it is not null or empty.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown in the event that the current sequence is empty.
        /// </exception>
        public static async Task<T> FirstAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var enumerator = source.GetEnumerator())
            {
                if (await enumerator.MoveNextAsync(token).ConfigureAwait(false))
                {
                    return enumerator.Current;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Get the first element in the current sequence that satisfies a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A condition to match against every element in the current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>The first element in the current sequence if it is not null or empty.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence or given predicate is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown in the event that the current sequence contains zero elements that match the given condition.
        /// </exception>
        public static Task<T> FirstAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, bool> predicate,
            CancellationToken token = default(CancellationToken)
            )
        {
            return source.Where(predicate).FirstAsync(token);
        }

        /// <summary>
        /// Get the first element of the current sequence or the default value for the element
        /// type if the sequence is empty.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// The first element in the current sequence or the default value for the
        /// element type if the current sequence is empty.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static async Task<T> FirstOrDefaultAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var enumerator = source.GetEnumerator())
            {
                if (await enumerator.MoveNextAsync(token).ConfigureAwait(false))
                {
                    return enumerator.Current;
                }
                else
                {
                    return default(T);
                }
            }
        }

        /// <summary>
        /// Get the first element of the current array that satisfies a given condition
        /// or the default value of the element type if the current sequence is empty.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A condition to match every element in the current sequence against.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// The first element in the current sequence that matches the given condition
        /// or the default value of the element type if the current sequence is empty.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence or given predicate is null.
        /// </exception>
        public static Task<T> FirstOrDefaultAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, bool> predicate,
            CancellationToken token = default(CancellationToken)
            )
        {
            return source.Where(predicate).FirstOrDefaultAsync(token);
        }

        /// <summary>
        /// Get the last element present in the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// The last element in the current sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown in the event that the current sequence is empty.
        /// </exception>
        public static async Task<T> LastAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var enumerator = source.GetEnumerator())
            {
                T current;

                if (false == await enumerator.MoveNextAsync(token).ConfigureAwait(false))
                {
                    throw new InvalidOperationException();
                }

                do
                {
                    current = enumerator.Current;

                } while (await enumerator.MoveNextAsync(token).ConfigureAwait(false));

                return current;
            }
        }

        /// <summary>
        /// Get the last element present in the current sequence that matches a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A condition to match every element of the current sequence against.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// The last element of the current sequence that matches the given condition.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence or given predicate is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown in the event that the current sequence contains zero elements that match the given condition.
        /// </exception>
        public static Task<T> LastAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, bool> predicate,
            CancellationToken token = default(CancellationToken)
            )
        {
            return source.Where(predicate).LastAsync(token);
        }

        /// <summary>
        /// Get the last value in the current sequence, or the default value of
        /// the element type if the current sequence is empty.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// The last value in the current sequence or the default value of the
        /// element type if the current sequence is empty.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static async Task<T> LastOrDefaultAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var enumerator = source.GetEnumerator())
            {
                T current;

                if (false == await enumerator.MoveNextAsync(token).ConfigureAwait(false))
                {
                    return default(T);
                }

                do
                {
                    current = enumerator.Current;

                } while (await enumerator.MoveNextAsync(token).ConfigureAwait(false));

                return current;
            }
        }

        /// <summary>
        /// Get the last element in the current sequence that matches a given condition
        /// or the default value of the element type if the current sequence contains
        /// zero elements that match the given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A condition to match against every element in the current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// The last element in the current sequence that matches the given condition or the default value
        /// of the element type if no elements in the current sequence match the given condition.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the source or predicate is null.
        /// </exception>
        public static Task<T> LastOrDefaultAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, bool> predicate,
            CancellationToken token = default(CancellationToken)
            )
        {
            return source.Where(predicate).LastOrDefaultAsync(token);
        }

        /// <summary>
        /// Filter the current sequence of elements into only those elements that are of a given type.
        /// </summary>
        /// <typeparam name="TBase">The less-derived type of the elements in the current sequence.</typeparam>
        /// <typeparam name="TDerived">The more-derived type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <returns>
        /// A sequence of only those elements of the current sequence that are of the given derived type.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static IAsyncEnumerable<TDerived> OfType<TBase, TDerived>(
            this IAsyncEnumerable<TBase> source
            )
        {
            Type type = typeof(TDerived);

            return source.Where(x => x.GetType().IsAssignableFrom(type))
                         .Select(x => (TDerived)Convert.ChangeType(x, type));
        }

        /// <summary>
        /// Project each element in the current sequence into a new form.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the current sequence.</typeparam>
        /// <typeparam name="TResult">
        /// The type of the value that each element will be projected as.
        /// </typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>
        /// A new sequence whose elements are the result of invoking the transform function
        /// on each element of the current sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current sequence or the given transform function is null
        /// </exception>
        public static IAsyncEnumerable<TResult> Select<TSource, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TResult> selector
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return new ProjectedSequence<TSource, TResult>(source, selector);
        }

        /// <summary>
        /// Project each element in the current sequence into a new form.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the current sequence.</typeparam>
        /// <typeparam name="TResult">
        /// The type of the value that each element will be projected as.
        /// </typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="selector">An asynchronous transform function to apply to each element.</param>
        /// <returns>
        /// A new sequence whose elements are the result of invoking the transform function
        /// on each element of the current sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current sequence or the given transform function is null
        /// </exception>
        public static IAsyncEnumerable<TResult> Select<TSource, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, CancellationToken, Task<TResult>> selector
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return new ProjectedSequenceAsync<TSource, TResult>(source, selector);
        }

        /// <summary>
        /// Compare the current sequence to another for equality. The default comparison strategy
        /// for the element type of the current sequence will be used.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="target">Another sequence to compare to the current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// True if the source and other sequence are of the same length and the elements at each
        /// sequence's corresponding positions are equal; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence or given target sequence is null.
        /// </exception>
        public static Task<bool> SequenceEqualsAsync<T>(
            this IAsyncEnumerable<T> source,
            IAsyncEnumerable<T> target,
            CancellationToken token = default(CancellationToken)
            )
        {
            return target.SequenceEqualsAsync(source, EqualityComparer<T>.Default, token);
        }

        /// <summary>
        /// Compare the current sequence to another for equality.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="target">Another sequence to compare to the current sequence.</param>
        /// <param name="comparer">An explicit element comparison strategy.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// True if the source and other sequence are of the same length and the elements at each
        /// sequence's corresponding positions are equal; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence, given target sequence or given comparer is null.
        /// </exception>
        public static async Task<bool> SequenceEqualsAsync<T>(
            this IAsyncEnumerable<T> source,
            IAsyncEnumerable<T> target,
            IEqualityComparer<T> comparer,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (comparer is null)
            {
                throw new ArgumentNullException(nameof(comparer));
            }

            using (var a = source.GetEnumerator())
            using (var b = target.GetEnumerator())
            {
                while (await a.MoveNextAsync(token).ConfigureAwait(false))
                {
                    if (false == await b.MoveNextAsync(token).ConfigureAwait(false))
                    {
                        return false;
                    }

                    if (!comparer.Equals(a.Current, b.Current))
                    {
                        return false;
                    }
                }

                return false == await b.MoveNextAsync(token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get the only element from the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// The first and only element in the current sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown in the event that the current sequence is emtpy or contains more than a single element.
        /// </exception>
        public static async Task<T> SingleAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var enumerator = source.GetEnumerator())
            {
                if (false == await enumerator.MoveNextAsync(token).ConfigureAwait(false))
                {
                    throw new InvalidOperationException("Sequence contains no elements.");
                }

                var element = enumerator.Current;

                if (await enumerator.MoveNextAsync(token).ConfigureAwait(false))
                {
                    throw new InvalidOperationException("Sequence contains more than one element.");
                }

                return element;
            }
        }

        /// <summary>
        /// Get the only element from the current sequence that matches a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">
        /// A condition to match each element of the current sequence against.
        /// </param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// The only element of the current sequence that matches the given condition.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence or given predicate is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown in the event that the current sequence contains zero element that match the given condition,
        /// or if the current sequence contains more than one element that satisfies the given condition.
        /// </exception>
        public static Task<T> SingleAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, bool> predicate,
            CancellationToken token = default(CancellationToken)
            )
        {
            return source.Where(predicate).SingleAsync(token);
        }

        /// <summary>
        /// Get the only element from the current sequence or the default value of the
        /// element type if the current sequence is empty.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// The only element in the current sequence or the default value of the element type
        /// if the current sequence is empty.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown in the event that the current sequence contains more than one element.
        /// </exception>
        public static async Task<T> SingleOrDefaultAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default(CancellationToken)
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            using (var enumerator = source.GetEnumerator())
            {
                if (false == await enumerator.MoveNextAsync(token).ConfigureAwait(false))
                {
                    return default(T);
                }

                var element = enumerator.Current;

                if (await enumerator.MoveNextAsync(token).ConfigureAwait(false))
                {
                    throw new InvalidOperationException("Sequence contains more than one element.");
                }

                return element;
            }
        }

        /// <summary>
        /// Get the only element from the current sequence that satisfies a given condition, or the default
        /// value of the element type if no elements in the current sequence satisfy the given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">
        /// A condition to apply to each element in the current sequence.
        /// </param>
        /// <param name="token">A cancellation instruction.</param>
        /// <returns>
        /// The only element in the current sequence that satisfies the given condition.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current sequence or the given condition are null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown in the event that the current sequence contains more than one element
        /// that satisfies the given condition.
        /// </exception>
        public static Task<T> SingleOrDefaultAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, bool> predicate,
            CancellationToken token = default(CancellationToken)
            )
        {
            return source.Where(predicate).SingleOrDefaultAsync(token);
        }

        /// <summary>
        /// Skip a given number of elements from the start of the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="amount">
        /// A value corresponding to the number of elements to skip.
        /// </param>
        /// <returns>
        /// A new sequence that contains elements from the current sequence that occur
        /// after skipping the given number of elements.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static IAsyncEnumerable<T> Skip<T>(this IAsyncEnumerable<T> source, ulong amount)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new SkippedSequence<T>(source, amount);
        }

        /// <summary>
        /// Return a given number of contiguous elements from the start of the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="count">
        /// A value corresponding to the number of contiguous elements
        /// to return from the start of the current sequence.
        /// </param>
        /// <returns>
        /// A new sequence that contains the given number of contiguous elements
        /// from the start of the current sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static IAsyncEnumerable<T> Take<T>(this IAsyncEnumerable<T> source, ulong count)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new LimitedSequence<T>(source, count);
        }

        /// <summary>
        /// Filter the current sequence using a given predicate.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">
        /// A condition against which every element in the current sequence will be matched.
        /// </param>
        /// <returns>
        /// A new sequence consisting only of elements from the current sequence that
        /// match the given predicate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current sequence of the given predicate is null.
        /// </exception>
        public static IAsyncEnumerable<T> Where<T>(this IAsyncEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return new FilteredSequence<T>(source, predicate);
        }

        /// <summary>
        /// Apply a given function to the corresponding elements of the current sequence and another sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the current sequence.</typeparam>
        /// <typeparam name="TOther">The type of the elements in the other sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="other">Another sequence to zip into the current sequence.</param>
        /// <param name="selector">
        /// A function to apply to the corresponding elements in the current and other sequences.
        /// </param>
        /// <returns>
        /// A sequence of elements zipped together from the current and another sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence, other sequence or selector is null.
        /// </exception>
        public static IAsyncEnumerable<TResult> Zip<TSource, TOther, TResult>(
            this IAsyncEnumerable<TSource> source,
            IAsyncEnumerable<TOther> other,
            Func<TSource, TOther, TResult> selector
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return new ZippedSequence<TSource, TOther, TResult>(source, other, selector);
        }

        /// <summary>
        /// Apply a given function to the corresponding elements of the current sequence and another sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the current sequence.</typeparam>
        /// <typeparam name="TOther">The type of the elements in the other sequence.</typeparam>
        /// <typeparam name="TResult">The type of the elements in the resulting sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="other">Another sequence to zip into the current sequence.</param>
        /// <param name="selector">
        /// A function to apply to the corresponding elements in the current and other sequences.
        /// </param>
        /// <returns>
        /// A sequence of elements zipped together from the current and another sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence, other sequence or selector is null.
        /// </exception>
        public static IAsyncEnumerable<TResult> Zip<TSource, TOther, TResult>(
            this IAsyncEnumerable<TSource> source,
            IAsyncEnumerable<TOther> other,
            Func<TSource, TOther, CancellationToken, Task<TResult>> selector
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other is null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            if (selector is null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            return new ZippedSequenceAsync<TSource, TOther, TResult>(source, other, selector);
        }
    }
}