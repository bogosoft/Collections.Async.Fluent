using Bogosoft.Testing.Objects;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class SkipMethodTests
    {
        [TestCase]
        public async Task SkipBypassesGivenNumberOfElements()
        {
            var count = 64;
            var ints = Integer.RandomSequence(256).ToArray();
            var source = ints.ToAsyncEnumerable();

            await using var a = source.SkipAsync(count).GetAsyncEnumerator();
            using var b = ints.Skip(count).GetEnumerator();

            while (await a.MoveNextAsync())
            {
                b.MoveNext().ShouldBeTrue();

                b.Current.ShouldBe(a.Current);
            }

            b.MoveNext().ShouldBeFalse();
        }

        [TestCase]
        public async Task SkipReturnsEmptySequenceWhenMoreItemsAreSkippedThanArePresentInASequence()
        {
            long size = 16;

            var sequence = Enumerable.Range(0, (int)size).ToAsyncEnumerable();

            var count = await sequence.SkipAsync(size).CountAsync();

            count.ShouldBe(0);
        }

        [TestCase]
        public void SkipThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            source.SkipAsync(16)
                  .ConsumeAsync()
                  .ShouldThrow<ArgumentNullException>();
        }
    }
}