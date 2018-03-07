using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent
{
    sealed class DistinctSequence<T> : IAsyncEnumerable<T>
    {
        class Enumerator : IAsyncEnumerator<T>
        {
            IAsyncEnumerator<T> source;
            HashSet<T> values;

            public T Current => source.Current;

            internal Enumerator(IAsyncEnumerator<T> source, IEqualityComparer<T> comparer)
            {
                this.source = source;
                this.values = new HashSet<T>(comparer);
            }

            public void Dispose()
            {
                source?.Dispose();
            }

            public async Task<bool> MoveNextAsync(CancellationToken token)
            {
                while (await source.MoveNextAsync(token))
                {
                    if (values.Contains(source.Current))
                    {
                        continue;
                    }
                    else
                    {
                        values.Add(source.Current);

                        return true;
                    }
                }

                return false;
            }
        }

        IEqualityComparer<T> comparer;
        IAsyncEnumerable<T> source;

        internal DistinctSequence(IAsyncEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            this.comparer = comparer;
            this.source = source;
        }

        public IAsyncEnumerator<T> GetEnumerator() => new Enumerator(source.GetEnumerator(), comparer);
    }
}