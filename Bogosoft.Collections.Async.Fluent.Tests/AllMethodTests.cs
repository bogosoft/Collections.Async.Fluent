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
    public class AllMethodTests
    {
        [TestCase]
        public async Task AllReturnsFalseWhenAtLeastOneItemInASequenceDoesNotMatchAGivenConditionAsync()
        {
            var ints = Integer.RandomSequence(256).Where(Integer.Odd).Concat(new int[] { 2 }).ToArray();

            ints.All(Integer.Odd).ShouldBeFalse();

            ints.Any(Integer.Even).ShouldBeTrue();

            await ints.ToAsyncEnumerable().AllAsync(Integer.Odd).ShouldBeFalseAsync();

            await ints.ToAsyncEnumerable().AnyAsync(Integer.Even).ShouldBeTrueAsync();
        }

        [TestCase]
        public async Task AllReturnsTrueWhenItemsInASequenceMatchAGivenConditionAsync()
        {
            var ints = Integer.RandomSequence(256).Where(Integer.Even).ToArray();

            ints.All(Integer.Even).ShouldBeTrue();

            await ints.ToAsyncEnumerable().AllAsync(Integer.Even).ShouldBeTrueAsync();
        }

        [TestCase]
        public async Task AllThrowsArgumentNullExceptionWhenPredicateIsNull()
        {
            var source = Enumerable.Range(0, 16).ToAsyncEnumerable();

            source.ShouldNotBeNull();

            await source.AllAsync(null).ShouldThrowAsync<bool, ArgumentNullException>();
        }

        [TestCase]
        public async Task AllThrowsArgumentNullExceptionWhenSourceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            await source.AllAsync(x => x % 2 == 0).ShouldThrowAsync<bool, ArgumentNullException>();
        }
    }
}