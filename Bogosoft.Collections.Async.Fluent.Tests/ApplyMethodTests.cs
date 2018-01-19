using Bogosoft.Testing.Objects;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class ApplyMethodTests
    {
        [TestCase]
        public async Task ApplyCallsFunctionWhenSourceSequenceAndActionAreNotNull()
        {
            var source = Integer.RandomSequence(16).ToArray();

            var count = 0;

            await source.ToAsyncEnumerable().ApplyAsync(x => ++count);

            count.ShouldBe(source.Length);

            count = 0;

            Func<int, CancellationToken, Task> action = async (i, t) =>
            {
                await Task.Delay(1);

                ++count;
            };

            await source.ToAsyncEnumerable().ApplyAsync(action);

            count.ShouldBe(source.Length);
        }

        [TestCase]
        public async Task ApplyThrowsArgumentNullExceptionWhenActionIsNull()
        {
            IAsyncEnumerable<int> source = null;

            await source.ApplyAsync(null as Action<int>).ShouldThrowAsync<ArgumentNullException>();

            Func<int, CancellationToken, Task> action = null;

            await source.ApplyAsync(action).ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task ApplyThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            var count = 0;

            await (null as IAsyncEnumerable<int>).ApplyAsync(x => count += x).ShouldThrowAsync<ArgumentNullException>();

            Func<int, CancellationToken, Task> action = async (i, t) =>
            {
                await Task.Delay(10, t);

                count += i;
            };

            await (null as IAsyncEnumerable<int>).ApplyAsync(action).ShouldThrowAsync<ArgumentNullException>();
        }
    }
}