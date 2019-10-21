using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class PrependMethodTests
    {
        static async ValueTask<int> CountElementAsync<T>(IAsyncEnumerable<T> source)
        {
            var count = 0;

            await foreach (var x in source)
            {
                count += 1;
            }

            return count;
        }

        static async Task<T> GetFirstElementAsync<T>(IAsyncEnumerable<T> source)
        {
            await using var enumerator = source.GetAsyncEnumerator();

            await enumerator.MoveNextAsync().ShouldBeTrueAsync();

            return enumerator.Current;
        }

        [TestCase]
        public async Task AttemptingToPrependAnElementToANullSequenceThrowsArgumentNullException()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            await source.PrependAsync(3)
                        .ConsumeAsync()
                        .ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task AttemptingToPrependANullSequenceToANonNullSequenceThrowsAgrumentNullException()
        {
            var source = new int[] { 1, 3, 5, 7, 9 }.ToAsyncEnumerable();

            int[] a = null;

            source.ShouldNotBeNull();

            a.ShouldBeNull();

            await source.PrependAsync(a)
                        .ConsumeAsync()
                        .ShouldThrowAsync<ArgumentNullException>();

            IAsyncEnumerable<int> b = null;

            source.ShouldNotBeNull();

            b.ShouldBeNull();

            await source.PrependAsync(b)
                        .ConsumeAsync()
                        .ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task AttemptingToPrependASequenceToANullSequenceThrowsArgumentNullException()
        {
            IAsyncEnumerable<int> source = null;

            int[] a = { 0, 2, 4, 6, 8 };

            source.ShouldBeNull();

            a.ShouldNotBeNull();

            await source.PrependAsync(a)
                        .ConsumeAsync()
                        .ShouldThrowAsync<ArgumentNullException>();

            var b = a.ToAsyncEnumerable();

            b.ShouldNotBeNull();
            b.ShouldNotBeAssignableTo<IEnumerable<int>>();
            b.ShouldBeAssignableTo<IAsyncEnumerable<int>>();

            await source.PrependAsync(b)
                        .ConsumeAsync()
                        .ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task EitherSequenceTypeCanBePrependedToAsyncSequence()
        {
            byte[] a = { 5, 6, 7, 8, 9 };
            byte[] b = { 0, 1, 2, 3, 4 };

            var source = a.ToAsyncEnumerable();

            await CountElementAsync(source).ShouldBeAsync(a.Length);

            await GetFirstElementAsync(source).ShouldBeAsync(a[0]);

            b.ShouldBeAssignableTo<IEnumerable<byte>>();

            source = source.PrependAsync(b);

            await CountElementAsync(source).ShouldBeAsync(a.Length + b.Length);

            await GetFirstElementAsync(source).ShouldBeAsync(b[0]);

            source = a.ToAsyncEnumerable();

            await CountElementAsync(source).ShouldBeAsync(a.Length);

            await GetFirstElementAsync(source).ShouldBeAsync(a[0]);

            var c = b.ToAsyncEnumerable();

            await GetFirstElementAsync(c).ShouldBeAsync(b[0]);

            source = source.PrependAsync(c);

            await CountElementAsync(source).ShouldBeAsync(a.Length + b.Length);

            await GetFirstElementAsync(source).ShouldBeAsync(b[0]);
        }

        [TestCase]
        public async Task ElementCanBePrependedToSequence()
        {
            int[] ints = { 2, 4, 6, 8 };

            var source = ints.ToAsyncEnumerable();

            await CountElementAsync(source).ShouldBeAsync(ints.Length);

            await GetFirstElementAsync(source).ShouldBeAsync(ints[0]);

            var first = 0;

            source = source.PrependAsync(first);

            await CountElementAsync(source).ShouldBeAsync(ints.Length + 1);

            await GetFirstElementAsync(source).ShouldBeAsync(first);
        }
    }
}