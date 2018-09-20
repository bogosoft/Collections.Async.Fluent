using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent
{
    class DelegateAppliedSequence<T> : IAsyncEnumerable<T>
    {
        class Enumerator : IAsyncEnumerator<T>
        {
            Action<T> action;
            IAsyncEnumerator<T> source;

            public T Current
            {
                get
                {
                    var current = source.Current;

                    action.Invoke(current);

                    return current;
                }
            }

            internal Enumerator(IAsyncEnumerator<T> source, Action<T> action)
            {
                this.action = action;
                this.source = source;
            }

            public void Dispose()
            {
                source.Dispose();
            }

            public Task<bool> MoveNextAsync(CancellationToken token) => source.MoveNextAsync(token);
        }

        Action<T> action;
        IAsyncEnumerable<T> source;

        internal DelegateAppliedSequence(IAsyncEnumerable<T> source, Action<T> action)
        {
            this.action = action;
            this.source = source;
        }

        public IAsyncEnumerator<T> GetEnumerator() => new Enumerator(source.GetEnumerator(), action);
    }
}