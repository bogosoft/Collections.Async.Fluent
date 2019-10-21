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
    public class ConcatMethodTests
    {
        [TestCase]
        public async Task ConcatCombinesTwoSequencesCorrectly()
        {
            var source = new int[] { 0, 1, 2, 3, 4 };
            var target = new int[] { 5, 6, 7, 8, 9 };

            var combined = source.ToAsyncEnumerable().ConcatAsync(target.ToAsyncEnumerable());

            combined.ShouldBeAssignableTo<IAsyncEnumerable<int>>();

            await using var a = combined.GetAsyncEnumerator();
            using var b = source.Concat(target).GetEnumerator();

            while (await a.MoveNextAsync())
            {
                b.MoveNext().ShouldBeTrue();

                a.Current.ShouldBe(b.Current);
            }

            b.MoveNext().ShouldBeFalse();
        }

        [TestCase]
        public void ConcatThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null, target = Integer.RandomSequence(16).ToAsyncEnumerable();

            source.ShouldBeNull();

            target.ShouldNotBeNull();

            source.ConcatAsync(target)
                  .ConsumeAsync()
                  .ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void ConcatThrowsArgumentNullExceptionWhenTargetSequenceIsNull()
        {
            IAsyncEnumerable<int> source = Integer.RandomSequence(16).ToAsyncEnumerable(), target = null;

            source.ShouldNotBeNull();

            target.ShouldBeNull();

            source.ConcatAsync(target)
                  .ConsumeAsync()
                  .ShouldThrow<ArgumentNullException>();
        }
    }
}