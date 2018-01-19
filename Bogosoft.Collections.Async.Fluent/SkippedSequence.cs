using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent
{
    class SkippedSequence<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
    {
        ulong amount, count = 0;
        IAsyncEnumerator<T> enumerator;
        IAsyncEnumerable<T> source;

        public T Current => enumerator.Current;

        internal SkippedSequence(IAsyncEnumerable<T> source, ulong amount)
        {
            this.amount = amount;
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
            while (count++ < amount && await enumerator.MoveNextAsync(token))
            {
            }

            return await enumerator.MoveNextAsync(token);
        }
    }
}