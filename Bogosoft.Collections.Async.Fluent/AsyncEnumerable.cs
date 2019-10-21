using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent
{
    /// <summary>
    /// Provides static members for working with <see cref="IAsyncEnumerable{T}"/> types.
    /// </summary>
    public static class AsyncEnumerable
    {
        /// <summary>
        /// Determine whether all elements in a sequence satsify a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A predicate to apply to each element in the sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// True if every element in the current sequence satisfies the given conidition; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the source sequence or predicate is null.
        /// </exception>
        public static async ValueTask<bool> AllAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, bool> predicate,
            CancellationToken token = default
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

            await foreach (var x in source.WithCancellation(token))
            {
                if (!predicate.Invoke(x))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determine whether the current sequence contains any elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// True if the current sequence contains at least one element; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static async ValueTask<bool> AnyAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            await using var enumerator = source.GetAsyncEnumerator(token);

            return await enumerator.MoveNextAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Determine if at least one element in the current sequence satisfies a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A predicate to be applied to all elements in the current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// True if at least one element in the current sequence satisfies the given condition; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the given source or predicate is null.
        /// </exception>
        public static ValueTask<bool> AnyAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, bool> predicate,
            CancellationToken token = default
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

            return source.WhereAsync(predicate, token).AnyAsync();
        }

        /// <summary>
        /// Append an element to the end of the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="last">An item to append to the end of the current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>The current sequence with the given element appended to it.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static async IAsyncEnumerable<T> AppendAsync<T>(
            this IAsyncEnumerable<T> source,
            T last,
            [EnumeratorCancellation]
            CancellationToken token = default
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                yield return x;
            }

            yield return last;
        }

        /// <summary>
        /// Append a synchronous sequence of elements to the end of the current sequence. To append an
        /// asynchronous sequence, use
        /// <see cref="ConcatAsync{T}(IAsyncEnumerable{T}, IAsyncEnumerable{T}, CancellationToken)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="other">A sequence of elements to be appended to the current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// The current sequence with the given sequence appended to it.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current or given sequence is null.
        /// </exception>
        public static async IAsyncEnumerable<T> AppendAsync<T>(
            this IAsyncEnumerable<T> source,
            IEnumerable<T> other,
            [EnumeratorCancellation]
            CancellationToken token = default
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

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                yield return x;
            }

            foreach (var y in other)
            {
                yield return y;
            }
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
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// The current sequence configured to invoke an action on each element of the
        /// current sequence once enumeration has started.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current sequence or the given action are null.
        /// </exception>
        public static async Task ApplyAsync<T>(
            this IAsyncEnumerable<T> source,
            Action<T> action,
            CancellationToken token = default
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

            await foreach (var x in source.WithCancellation(token))
            {
                action.Invoke(x);
            }
        }

        /// <summary>
        /// Enumerate the current sequence, applying a given action to each enumerated element.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="action">An action to apply to each element in the current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>A task representing an asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence or given action is null.
        /// </exception>
        public static async Task ApplyAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, CancellationToken, Task> action,
            CancellationToken token = default
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

            await foreach (var x in source.WithCancellation(token))
            {
                await action.Invoke(x, token);
            }
        }

        /// <summary>
        /// Combine the current sequence with another.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="target">A sequence of elements append to the current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A single, concatenated sequence consisting of the current sequence and another sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current or given sequence is null.
        /// </exception>
        public static async IAsyncEnumerable<T> ConcatAsync<T>(
            this IAsyncEnumerable<T> source,
            IAsyncEnumerable<T> target,
            [EnumeratorCancellation]
            CancellationToken token = default
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

            await foreach (var x in source.WithCancellation(token))
            {
                yield return x;
            }

            await foreach (var x in target.WithCancellation(token))
            {
                yield return x;
            }
        }

        /// <summary>
        /// Determine whether the current sequence contains a given item
        /// using the default equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="item">An item to look for in the current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// True if the current sequence contains the given item; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static ValueTask<bool> ContainsAsync<T>(
            this IAsyncEnumerable<T> source,
            T item,
            CancellationToken token = default
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
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// True if the current sequence contains the given item; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence or given comparer is null.
        /// </exception>
        public static async ValueTask<bool> ContainsAsync<T>(
            this IAsyncEnumerable<T> source,
            T item,
            IEqualityComparer<T> comparer,
            CancellationToken token = default
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

            await foreach (var x in source.WithCancellation(token))
            {
                if (comparer.Equals(x, item))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Count the number of elements in the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A value corresponding to the number of elements in the current sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static async ValueTask<long> CountAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            long count = 0;

            await foreach (var x in source.WithCancellation(token))
            {
                count += 1;
            }

            return count;
        }

        /// <summary>
        /// Count the number of elements in the current sequence that satisfy a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A condition to check each element of the current sequence against.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A value corresponding to the number of elements in the current sequence
        /// that satisfy the given condition.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence or given predicate is null.
        /// </exception>
        public static ValueTask<long> CountAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, bool> predicate,
            CancellationToken token = default
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

            return source.WhereAsync(predicate, token).CountAsync();
        }

        /// <summary>
        /// Condense the current sequence into a sequence of only its distinct elements. Elements will be
        /// compared to one another using the <see cref="EqualityComparer{T}.Default"/> value.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>A sequence of distinct elements.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static IAsyncEnumerable<T> DistinctAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default
            )
        {
            return source.DistinctAsync(EqualityComparer<T>.Default, token);
        }

        /// <summary>
        /// Condense the current sequence into a sequence of only its distinct elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="comparer">A strategy for comparing the equality of elements.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>A sequence of distinct elements.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current sequence or the given comparer is null.
        /// </exception>
        public static async IAsyncEnumerable<T> DistinctAsync<T>(
            this IAsyncEnumerable<T> source,
            IEqualityComparer<T> comparer,
            [EnumeratorCancellation]
            CancellationToken token = default
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

            var encountered = new HashSet<T>(comparer);

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                if (encountered.Add(x))
                {
                    yield return x;
                }
            }
        }

        /// <summary>
        /// Get the first element in the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>The first element in the current sequence if it is not null or empty.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown in the event that the current sequence is empty.
        /// </exception>
        public static async Task<T> FirstAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            await using var enumerator = source.GetAsyncEnumerator(token);

            if (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                return enumerator.Current;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Get the first element in the current sequence that satisfies a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A condition to match against every element in the current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
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
            CancellationToken token = default
            )
        {
            return source.WhereAsync(predicate, token).FirstAsync();
        }

        /// <summary>
        /// Get the first element of the current sequence or the default value for the element
        /// type if the sequence is empty.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// The first element in the current sequence or the default value for the
        /// element type if the current sequence is empty.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static async Task<T> FirstOrDefaultAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            await using var enumerator = source.GetAsyncEnumerator(token);

            return await enumerator.MoveNextAsync().ConfigureAwait(false)
                 ? enumerator.Current
                 : default;
        }

        /// <summary>
        /// Get the first element of the current array that satisfies a given condition
        /// or the default value of the element type if the current sequence is empty.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A condition to match every element in the current sequence against.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
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
            CancellationToken token = default
            )
        {
            return source.WhereAsync(predicate, token).FirstOrDefaultAsync(token);
        }

        /// <summary>
        /// Get the last element present in the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
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
            CancellationToken token = default
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            T last = default;

            await using var enumerator = source.GetAsyncEnumerator(token);

            if (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                do
                {
                    last = enumerator.Current;

                } while (await enumerator.MoveNextAsync().ConfigureAwait(false));
            }
            else
            {
                throw new InvalidOperationException();
            }

            return last;
        }

        /// <summary>
        /// Get the last element present in the current sequence that matches a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A condition to match every element of the current sequence against.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
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
            CancellationToken token = default
            )
        {
            return source.WhereAsync(predicate, token).LastAsync(token);
        }

        /// <summary>
        /// Get the last value in the current sequence, or the default value of
        /// the element type if the current sequence is empty.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// The last value in the current sequence or the default value of the
        /// element type if the current sequence is empty.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static async Task<T> LastOrDefaultAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            T last = default;

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                last = x;
            }

            return last;
        }

        /// <summary>
        /// Get the last element in the current sequence that matches a given condition
        /// or the default value of the element type if the current sequence contains
        /// zero elements that match the given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">A condition to match against every element in the current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
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
            CancellationToken token = default
            )
        {
            return source.WhereAsync(predicate, token).LastOrDefaultAsync(token);
        }

        /// <summary>
        /// Filter the current sequence of elements into only those elements that are of a given type.
        /// </summary>
        /// <typeparam name="TBase">The less-derived type of the elements in the current sequence.</typeparam>
        /// <typeparam name="TDerived">The more-derived type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A sequence of only those elements of the current sequence that are of the given derived type.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static async IAsyncEnumerable<TDerived> OfTypeAsync<TBase, TDerived>(
            this IAsyncEnumerable<TBase> source,
            [EnumeratorCancellation]
            CancellationToken token = default
            )
            where TDerived : TBase
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                if (x is TDerived d)
                {
                    yield return d;
                }
            }
        }

        /// <summary>
        /// Prepend a given element to the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="first">An element to prepend to the current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>The current sequence with the given element prepended to it.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static async IAsyncEnumerable<T> PrependAsync<T>(
            this IAsyncEnumerable<T> source,
            T first,
            [EnumeratorCancellation]
            CancellationToken token = default
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            yield return first;

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                yield return x;
            }
        }

        /// <summary>
        /// Prepend a given sequence to the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="other">A sequence to prepend to the current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>The current sequence with the given sequence prepended to it.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current or given sequence is null.
        /// </exception>
        public static async IAsyncEnumerable<T> PrependAsync<T>(
            this IAsyncEnumerable<T> source,
            IAsyncEnumerable<T> other,
            [EnumeratorCancellation]
            CancellationToken token = default
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

            await foreach (var y in other.WithCancellation(token).ConfigureAwait(false))
            {
                yield return y;
            }

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                yield return x;
            }
        }

        /// <summary>
        /// Prepend a given sequence to the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="other">A sequence to prepend to the current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>The current sequence with the given sequence prepended to it.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current or given sequence is null.
        /// </exception>
        public static async IAsyncEnumerable<T> PrependAsync<T>(
            this IAsyncEnumerable<T> source,
            IEnumerable<T> other,
            [EnumeratorCancellation]
            CancellationToken token = default
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

            foreach (var y in other)
            {
                if (token.IsCancellationRequested)
                {
                    yield break;
                }

                yield return y;
            }

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                yield return x;
            }
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
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A new sequence whose elements are the result of invoking the transform function
        /// on each element of the current sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current sequence or the given transform function is null
        /// </exception>
        public static async IAsyncEnumerable<TResult> SelectAsync<TSource, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, TResult> selector,
            [EnumeratorCancellation]
            CancellationToken token = default
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

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                yield return selector.Invoke(x);
            }
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
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A new sequence whose elements are the result of invoking the transform function
        /// on each element of the current sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current sequence or the given transform function is null
        /// </exception>
        public static async IAsyncEnumerable<TResult> SelectAsync<TSource, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, CancellationToken, Task<TResult>> selector,
            [EnumeratorCancellation]
            CancellationToken token = default
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

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                yield return await selector.Invoke(x, token);
            }
        }

        /// <summary>
        /// Project each element of the current sequence to its own sequence and then
        /// flatten the projected sequences into a single sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the current sequence</typeparam>
        /// <typeparam name="TResult">The type of the elements of the final sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="selector">
        /// A transformation function to apply to each element of the current sequence.
        /// </param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A sequence consisting of each element projected to its own sequence and each of those
        /// sequences combined into a single, flattened sequence of elements.
        /// </returns>
        public static async IAsyncEnumerable<TResult> SelectManyAsync<TSource, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, IAsyncEnumerable<TResult>> selector,
            [EnumeratorCancellation]
            CancellationToken token = default
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

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                await foreach (var y in selector.Invoke(x).WithCancellation(token).ConfigureAwait(false))
                {
                    yield return y;
                }
            }
        }

        /// <summary>
        /// Project each element of the current sequence to its own sequence and then
        /// flatten the projected sequences into a single sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the current sequence</typeparam>
        /// <typeparam name="TResult">The type of the elements of the final sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="selector">
        /// A transformation function to apply to each element of the current sequence.
        /// </param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A sequence consisting of each element projected to its own sequence and each of those
        /// sequences combined into a single, flattened sequence of elements.
        /// </returns>
        public static async IAsyncEnumerable<TResult> SelectManyAsync<TSource, TResult>(
            this IAsyncEnumerable<TSource> source,
            Func<TSource, IEnumerable<TResult>> selector,
            [EnumeratorCancellation]
            CancellationToken token = default
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

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                foreach (var y in selector.Invoke(x))
                {
                    yield return y;
                }
            }
        }

        /// <summary>
        /// Project each element of the current sequence to its own sequence and then
        /// flatten the projected sequences into a single sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the current sequence</typeparam>
        /// <typeparam name="TResult">The type of the elements of the final sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="selector">
        /// A transformation function to apply to each element of the current sequence.
        /// </param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A sequence consisting of each element projected to its own sequence and each of those
        /// sequences combined into a single, flattened sequence of elements.
        /// </returns>
        public static async IAsyncEnumerable<TResult> SelectManyAsync<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, IAsyncEnumerable<TResult>> selector,
            [EnumeratorCancellation]
            CancellationToken token = default
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

            foreach (var x in source)
            {
                if (token.IsCancellationRequested)
                {
                    yield break;
                }

                await foreach (var y in selector.Invoke(x).WithCancellation(token).ConfigureAwait(false))
                {
                    yield return y;
                }
            }
        }

        /// <summary>
        /// Compare the current sequence to another for equality. The default comparison strategy
        /// for the element type of the current sequence will be used.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="target">Another sequence to compare to the current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// True if the source and other sequence are of the same length and the elements at each
        /// sequence's corresponding positions are equal; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence or given target sequence is null.
        /// </exception>
        public static ValueTask<bool> SequenceEqualsAsync<T>(
            this IAsyncEnumerable<T> source,
            IAsyncEnumerable<T> target,
            CancellationToken token = default
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
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// True if the source and other sequence are of the same length and the elements at each
        /// sequence's corresponding positions are equal; false otherwise.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence, given target sequence or given comparer is null.
        /// </exception>
        public static async ValueTask<bool> SequenceEqualsAsync<T>(
            this IAsyncEnumerable<T> source,
            IAsyncEnumerable<T> target,
            IEqualityComparer<T> comparer,
            CancellationToken token = default
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

            await using var a = source.GetAsyncEnumerator(token);
            await using var b = target.GetAsyncEnumerator(token);

            while (await a.MoveNextAsync().ConfigureAwait(false))
            {
                if (!await b.MoveNextAsync().ConfigureAwait(false))
                {
                    return false;
                }

                if (!comparer.Equals(a.Current, b.Current))
                {
                    return false;
                }
            }

            return !await b.MoveNextAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get the only element from the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
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
            CancellationToken token = default
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            await using var enumerator = source.GetAsyncEnumerator(token);

            if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                throw new InvalidOperationException("Sequence contains no elements.");
            }

            var element = enumerator.Current;

            if (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                throw new InvalidOperationException("Sequence contains more than one element.");
            }

            return element;
        }

        /// <summary>
        /// Get the only element from the current sequence that matches a given condition.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">
        /// A condition to match each element of the current sequence against.
        /// </param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
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
            CancellationToken token = default
            )
        {
            return source.WhereAsync(predicate, token).SingleAsync(token);
        }

        /// <summary>
        /// Get the only element from the current sequence or the default value of the
        /// element type if the current sequence is empty.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
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
            CancellationToken token = default
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            await using var enumerator = source.GetAsyncEnumerator(token);

            if (!await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                return default;
            }

            var element = enumerator.Current;

            if (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                throw new InvalidOperationException("Sequence contains more than one element.");
            }

            return element;
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
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
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
            CancellationToken token = default
            )
        {
            return source.WhereAsync(predicate, token).SingleOrDefaultAsync(token);
        }

        /// <summary>
        /// Skip a given number of elements from the start of the current sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="amount">
        /// A value corresponding to the number of elements to skip.
        /// </param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A new sequence that contains elements from the current sequence that occur
        /// after skipping the given number of elements.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static async IAsyncEnumerable<T> SkipAsync<T>(
            this IAsyncEnumerable<T> source,
            long amount,
            [EnumeratorCancellation]
            CancellationToken token = default
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            await using var enumerator = source.GetAsyncEnumerator(token);

            long skipped = 0;

            while (skipped < amount && await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                skipped += 1;
            }

            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                yield return enumerator.Current;
            }
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
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A new sequence that contains the given number of contiguous elements
        /// from the start of the current sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence is null.
        /// </exception>
        public static async IAsyncEnumerable<T> TakeAsync<T>(
            this IAsyncEnumerable<T> source,
            long count,
            [EnumeratorCancellation]
            CancellationToken token = default
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            long taken = 0;

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                if (taken++ >= count)
                {
                    break;
                }

                yield return x;
            }
        }

        /// <summary>
        /// Conver the current asynchronous sequence into an array.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>An array consisting of the elements of the current sequence.</returns>
        public static async Task<T[]> ToArrayAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default
            )
        {
            var length = 0;
            var result = new T[16];

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                if (length == result.Length)
                {
                    Array.Resize(ref result, length * 2);
                }

                result[length++] = x;
            }

            if (length < result.Length)
            {
                Array.Resize(ref result, length);
            }

            return result;
        }

        /// <summary>
        /// Convert the current synchronous sequence into an asynchronous sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current synchronous sequence.</param>
        /// <returns>The current synchronous sequence as an asynchronous sequence.</returns>
#pragma warning disable CS1998
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
#pragma warning restore CS1998
        {
            foreach (var x in source)
            {
                yield return x;
            }
        }

        /// <summary>
        /// Convert the current asynchronous sequence into a synchronous sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>The current asynchronous sequence as a synchronous sequence.</returns>
        public static IEnumerable<T> ToSynchronous<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            IAsyncEnumerator<T> enumerator = null;

            try
            {
                enumerator = source.GetAsyncEnumerator(token);

                while (enumerator.MoveNextAsync().GetAwaiter().GetResult())
                {
                    yield return enumerator.Current;
                }
            }
            finally
            {
                enumerator?.DisposeAsync().GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// Filter the current sequence using a given predicate.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the current sequence.</typeparam>
        /// <param name="source">The current sequence.</param>
        /// <param name="predicate">
        /// A condition against which every element in the current sequence will be matched.
        /// </param>
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A new sequence consisting only of elements from the current sequence that
        /// match the given predicate.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that either the current sequence of the given predicate is null.
        /// </exception>
        public static async IAsyncEnumerable<T> WhereAsync<T>(
            this IAsyncEnumerable<T> source,
            Func<T, bool> predicate,
            [EnumeratorCancellation]
            CancellationToken token = default
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

            await foreach (var x in source.WithCancellation(token).ConfigureAwait(false))
            {
                if (predicate.Invoke(x))
                {
                    yield return x;
                }
            }
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
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A sequence of elements zipped together from the current and another sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence, other sequence or selector is null.
        /// </exception>
        public static async IAsyncEnumerable<TResult> ZipAsync<TSource, TOther, TResult>(
            this IAsyncEnumerable<TSource> source,
            IAsyncEnumerable<TOther> other,
            Func<TSource, TOther, TResult> selector,
            [EnumeratorCancellation]
            CancellationToken token = default
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

            await using var a = source.GetAsyncEnumerator(token);
            await using var b = other.GetAsyncEnumerator(token);

            while (await a.MoveNextAsync().ConfigureAwait(false) && await b.MoveNextAsync().ConfigureAwait(false))
            {
                yield return selector.Invoke(a.Current, b.Current);
            }
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
        /// <param name="token">An opportunity to respond to a cancellation request.</param>
        /// <returns>
        /// A sequence of elements zipped together from the current and another sequence.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown in the event that the current sequence, other sequence or selector is null.
        /// </exception>
        public static async IAsyncEnumerable<TResult> ZipAsync<TSource, TOther, TResult>(
            this IAsyncEnumerable<TSource> source,
            IAsyncEnumerable<TOther> other,
            Func<TSource, TOther, CancellationToken, Task<TResult>> selector,
            [EnumeratorCancellation]
            CancellationToken token = default
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

            await using var a = source.GetAsyncEnumerator(token);
            await using var b = other.GetAsyncEnumerator(token);

            while (await a.MoveNextAsync().ConfigureAwait(false) && await b.MoveNextAsync().ConfigureAwait(false))
            {
                yield return await selector.Invoke(a.Current, b.Current, token);
            }
        }
    }

    /// <summary>
    /// Provides static members for working with <see cref="IAsyncEnumerable{T}"/> types.
    /// </summary>
    public static class AsyncEnumerable<T>
    {
        class EmptySequence : IAsyncEnumerable<T>
        {
            class Enumerator : IAsyncEnumerator<T>
            {
                public T Current => throw new InvalidOperationException();

                public ValueTask DisposeAsync() => new ValueTask();

                public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken token = default)
            {
                return new Enumerator();
            }
        }

        /// <summary>
        /// Get an empty asynchronous sequence.
        /// </summary>
        public static IAsyncEnumerable<T> Empty => new EmptySequence();
    }
}