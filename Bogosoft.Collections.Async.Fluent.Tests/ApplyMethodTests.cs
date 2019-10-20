using Bogosoft.Testing.Objects;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class ApplyMethodTests
    {
        [TestCase]
        public async Task ApplyAsyncCallsFunctionWhenSourceSequenceAndActionAreNotNull()
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
        public async Task ApplyAsyncThrowsArgumentNullExceptionWhenGivenActionIsNull()
        {
            var source = Integer.RandomSequence(16).ToAsyncEnumerable();

            source.ShouldNotBeNull();

            typeof(IAsyncEnumerable<int>).IsAssignableFrom(source.GetType()).ShouldBeTrue();

            await source.ApplyAsync(null as Action<int>).ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task ApplyAsyncThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            var count = 0;

            await (null as IAsyncEnumerable<int>).ApplyAsync(x => count += x).ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task ApplyCallsActionWhenActionDecoratedSequenceIsEnumerated()
        {
            int actual = 0, expected = 0;
            int[] ints;

            do
            {
                ints = Integer.RandomSequence(32, -32768, 32767).ToArray();

                expected = ints.Sum();

            } while (expected == 0);

            await ints.ToAsyncEnumerable().ApplyAsync(x => actual += x);

            actual.ShouldBe(expected);
        }

        [TestCase]
        public async Task ApplyThrowsArgumentNullExceptionWhenGivenActionIsNull()
        {
            var source = Integer.RandomSequence(16).ToAsyncEnumerable();

            Exception exception = null;

            try
            {
                source.ShouldNotBeNull();

                await source.ApplyAsync(null);
            }
            catch (Exception e)
            {
                exception = e;
            }

            exception.ShouldNotBeNull();

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [TestCase]
        public async Task ApplyThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<string> source = null;

            Exception exception = null;

            try
            {
                int length = 0;

                source.ShouldBeNull();

                await source.ApplyAsync(s => length += s.Length);
            }
            catch (Exception e)
            {
                exception = e;
            }

            exception.ShouldNotBeNull();

            exception.ShouldBeOfType<ArgumentNullException>();
        }
    }
}