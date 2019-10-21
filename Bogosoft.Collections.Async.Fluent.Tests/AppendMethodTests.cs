using Bogosoft.Testing.Objects;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class AppendMethodTests
    {
        static async ValueTask<int> CountElementsAsync<T>(IAsyncEnumerable<T> source)
        {
            var count = 0;

            await foreach (var x in source)
            {
                count += 1;
            }

            return count;
        }

        static async Task<T> GetLastElementAsync<T>(IAsyncEnumerable<T> source)
        {
            T last = default;

            await foreach (var x in source)
            {
                last = x;
            }

            return last;
        }

        [TestCase]
        public async Task AttemptingToAppendANonNullSequenceToANullSequenceThrowsArgumentNullException()
        {
            IAsyncEnumerable<int> source = null;

            int[] additional = { 0, 2, 4, 6, 8 };

            source.ShouldBeNull();

            additional.ShouldNotBeNull();

            additional.ShouldBeAssignableTo<IEnumerable<int>>();

            await source.AppendAsync(additional)
                        .ConsumeAsync()
                        .ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task AttemptingToAppendANullSequenceToANonNullSequenceThrowsArgumentNullException()
        {
            var source = new CelestialBody[0].ToAsyncEnumerable();

            IEnumerable<CelestialBody> additional = null;

            source.ShouldNotBeNull();

            additional.ShouldBeNull();

            await source.AppendAsync(additional)
                        .ConsumeAsync()
                        .ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task AttemptingToAppendASingleElementToANullSequenceThrowsArgumentNullException()
        {
            IAsyncEnumerable<string> source = null;

            source.ShouldBeNull();

            await source.AppendAsync("Hello, World!")
                        .ConsumeAsync()
                        .ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task ElementAppendsCorrectlyToSequence()
        {
            var ints = new int[] { 1, 2, 3 };

            var source = ints.ToAsyncEnumerable();

            source.ShouldBeAssignableTo<IAsyncEnumerable<int>>();

            await GetLastElementAsync(source).ShouldBeAsync(ints[^1]);

            source = source.AppendAsync(4);

            await GetLastElementAsync(source).ShouldBeAsync(4);
        }

        [TestCase]
        public async Task SynchronousSequenceAppendsCorrectlyToAsynchronousSequence()
        {
            var a = new bool[] { false, false, false };
            var b = new bool[] { true, true, true };

            var source = a.ToAsyncEnumerable();

            source.ShouldBeAssignableTo<IAsyncEnumerable<bool>>();

            await CountElementsAsync(source).ShouldBeAsync(a.Length);

            source = source.AppendAsync(b);

            await CountElementsAsync(source).ShouldBeAsync(a.Length + b.Length);

            await GetLastElementAsync(source).ShouldBeAsync(b[^1]);
        }
    }
}