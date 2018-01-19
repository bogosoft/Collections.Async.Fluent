using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent
{
    class ConcatenatedSequence<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
    {
        IAsyncEnumerable<T> a, b;
        IAsyncEnumerator<T> active, x, y;

        public T Current => active.Current;

        internal ConcatenatedSequence(IAsyncEnumerable<T> a, IAsyncEnumerable<T> b)
        {
            this.a = a;
            this.b = b;
        }

        public void Dispose()
        {
            x?.Dispose();
            y?.Dispose();
        }

        public IAsyncEnumerator<T> GetEnumerator()
        {
            x = a.GetEnumerator();
            y = b.GetEnumerator();

            return this;
        }

        public async Task<bool> MoveNextAsync(CancellationToken token)
        {
            return await (active = x).MoveNextAsync(token) || await (active = y).MoveNextAsync(token);
        }
    }
}