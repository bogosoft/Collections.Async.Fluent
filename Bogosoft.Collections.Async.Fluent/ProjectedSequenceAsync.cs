using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent
{
    class ProjectedSequenceAsync<TSource, TResult> : IAsyncEnumerable<TResult>, IAsyncEnumerator<TResult>
    {
        TResult buffer;
        IAsyncEnumerator<TSource> enumerator;
        Func<TSource, CancellationToken, Task<TResult>> selector;
        IAsyncEnumerable<TSource> source;

        public TResult Current => buffer;

        public ProjectedSequenceAsync(
            IAsyncEnumerable<TSource> source,
            Func<TSource, CancellationToken, Task<TResult>> selector
            )
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
            if (await enumerator.MoveNextAsync(token).ConfigureAwait(false))
            {
                buffer = await selector.Invoke(enumerator.Current, token).ConfigureAwait(false);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}