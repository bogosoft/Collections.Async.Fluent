using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent
{
    class ZippedSequenceAsync<TSource, TOther, TResult> :
        IAsyncEnumerable<TResult>,
        IAsyncEnumerator<TResult>
    {
        IAsyncEnumerable<TSource> a;
        IAsyncEnumerable<TOther> b;
        TResult buffer;
        IAsyncEnumerator<TSource> x;
        IAsyncEnumerator<TOther> y;
        Func<TSource, TOther, CancellationToken, Task<TResult>> selector;

        public TResult Current => buffer;

        internal ZippedSequenceAsync(
            IAsyncEnumerable<TSource> a,
            IAsyncEnumerable<TOther> b,
            Func<TSource, TOther, CancellationToken, Task<TResult>> selector
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
            Dispose();

            x = a.GetEnumerator();
            y = b.GetEnumerator();

            return this;
        }

        public async Task<bool> MoveNextAsync(CancellationToken token)
        {
            if (await x.MoveNextAsync(token).ConfigureAwait(false) && await y.MoveNextAsync(token).ConfigureAwait(false))
            {
                buffer = await selector.Invoke(x.Current, y.Current, token).ConfigureAwait(false);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}