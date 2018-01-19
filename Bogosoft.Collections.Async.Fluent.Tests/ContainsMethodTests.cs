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
    public class ContainsMethodTests
    {
        [TestCase]
        public async Task ContainsReturnsFalseWhenSequenceDoesNotContainGivenItem()
        {
            var ints = new int[] { 0, 2, 4, 6, 8 };
            var item = 1;

            ints.Contains(item).ShouldBeFalse();

            var source = ints.ToAsyncEnumerable();

            source.ShouldNotBeNull();

            source.ShouldBeAssignableTo<IAsyncEnumerable<int>>();

            await source.ContainsAsync(item).ShouldBeFalseAsync();
        }

        [TestCase]
        public async Task ContainsReturnsTrueWhenSequenceContainsGivenItem()
        {
            var ints = new int[] { 0, 2, 4, 6, 8 };
            var item = ints[0];

            ints.Contains(item).ShouldBeTrue();

            var source = ints.ToAsyncEnumerable();

            source.ShouldNotBeNull();

            source.ShouldBeAssignableTo<IAsyncEnumerable<int>>();

            await source.ContainsAsync(item).ShouldBeTrueAsync();
        }

        [TestCase]
        public async Task ContainsThrowsArgumentNullExceptionWhenComparerIsNull()
        {
            var source = Integer.RandomSequence(32).ToAsyncEnumerable();

            source.ShouldNotBeNull();

            source.ShouldBeAssignableTo<IAsyncEnumerable<int>>();

            IEqualityComparer<int> comparer = null;

            comparer.ShouldBeNull();

            await source.ContainsAsync(0, comparer).ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task ContainsThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            await source.ContainsAsync(0).ShouldThrowAsync<ArgumentNullException>();
        }
    }
}