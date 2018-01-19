using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent
{
    class FilteredSequence<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
    {
        IAsyncEnumerator<T> enumerator;
        Func<T, bool> qualifier;
        IAsyncEnumerable<T> source;

        public T Current => enumerator.Current;

        internal FilteredSequence(IAsyncEnumerable<T> source, Func<T, bool> qualifier)
        {
            this.qualifier = qualifier;
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
            while (true)
            {
                if (false == await enumerator.MoveNextAsync(token).ConfigureAwait(false))
                {
                    return false;
                }

                if (qualifier.Invoke(enumerator.Current))
                {
                    return true;
                }
            }
        }
    }
}