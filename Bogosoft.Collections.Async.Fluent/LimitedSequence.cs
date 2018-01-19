using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent
{
    class LimitedSequence<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
    {
        ulong count, limit;
        IAsyncEnumerator<T> enumerator;
        IAsyncEnumerable<T> source;

        public T Current => enumerator.Current;

        internal LimitedSequence(IAsyncEnumerable<T> source, ulong limit)
        {
            this.limit = limit;
            this.source = source;
        }

        public void Dispose()
        {
            enumerator?.Dispose();
        }

        public IAsyncEnumerator<T> GetEnumerator()
        {
            enumerator = source.GetEnumerator();

            return this;
        }

        public async Task<bool> MoveNextAsync(CancellationToken token)
        {
            return count++ < limit && await enumerator.MoveNextAsync(token).ConfigureAwait(false);
        }
    }
}