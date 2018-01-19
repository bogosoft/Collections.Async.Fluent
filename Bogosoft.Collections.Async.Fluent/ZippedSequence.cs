using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent
{
    class ZippedSequence<TSource, TOther, TResult> : IAsyncEnumerable<TResult>, IAsyncEnumerator<TResult>
    {
        IAsyncEnumerable<TSource> a;
        IAsyncEnumerable<TOther> b;
        IAsyncEnumerator<TSource> x;
        IAsyncEnumerator<TOther> y;
        Func<TSource, TOther, TResult> selector;

        public TResult Current => selector.Invoke(x.Current, y.Current);

        internal ZippedSequence(
            IAsyncEnumerable<TSource> a,
            IAsyncEnumerable<TOther> b,
            Func<TSource, TOther, TResult> selector
            )
        {
            this.a = a;
            this.b = b;
            this.selector = selector;
        }

        public void Dispose()
        {
            x?.Dispose();
            y?.Dispose();
        }

        public IAsyncEnumerator<TResult> GetEnumerator()
        {
            x = a.GetEnumerator();
            y = b.GetEnumerator();

            return this;
        }

        public async Task<bool> MoveNextAsync(CancellationToken token)
        {
            return await x.MoveNextAsync(token).ConfigureAwait(false)
                && await y.MoveNextAsync(token).ConfigureAwait(false);
        }
    }
}