using Bogosoft.Testing.Objects;
using NUnit.Framework;
using Shouldly;
using System;
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

            using (var a = source.Skip((ulong)count).GetEnumerator())
            using (var b = ints.Skip(count).GetEnumerator())
            {
                while (await a.MoveNextAsync())
                {
                    b.MoveNext().ShouldBeTrue();

                    b.Current.ShouldBe(a.Current);
                }

                b.MoveNext().ShouldBeFalse();
            }
        }

        [TestCase]
        public async Task SkipReturnsEmptySequenceWhenMoreItemsAreSkippedThanArePresentInASequence()
        {
            ulong size = 16;

            var sequence = Enumerable.Range(0, (int)size).ToAsyncEnumerable();

            var count = await sequence.Skip(size).CountAsync();

            count.ShouldBe((ulong)0);
        }

        [TestCase]
        public void SkipThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            Action test = () => source.Skip(16);

            test.ShouldThrow<ArgumentNullException>();
        }
    }
}