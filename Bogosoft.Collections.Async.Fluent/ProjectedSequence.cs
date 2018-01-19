using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent
{
    class ProjectedSequence<TSource, TResult> : IAsyncEnumerable<TResult>, IAsyncEnumerator<TResult>
    {
        IAsyncEnumerator<TSource> enumerator;
        Func<TSource, TResult> selector;
        IAsyncEnumerable<TSource> source;

        public TResult Current => selector.Invoke(enumerator.Current);

        internal ProjectedSequence(IAsyncEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            this.selector = selector;
            this.source = source;
        }

        public void Dispose()
        {
            enumerator?.Dispose();
        }

        public IAsyncEnumerator<TResult> GetEnumerator()
        {
            enumerator = source.GetEnumerator();

            return this;
        }

        public async Task<bool> MoveNextAsync(CancellationToken token)
        {
            return await enumerator.MoveNextAsync(token).ConfigureAwait(false);
        }
    }
}