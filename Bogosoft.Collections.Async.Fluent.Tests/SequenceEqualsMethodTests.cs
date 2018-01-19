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
    public class SequenceEqualsMethodTests
    {
        [TestCase]
        public async Task SequenceEqualsReturnsTrueWhenSequencesAreEqualLengthAndCorrespondingElementsAreEqual()
        {
            int[] a = Integer.RandomSequence(128).ToArray(), b = new int[a.Length];

            Array.Copy(a, b, a.Length);

            ReferenceEquals(a, b).ShouldBeFalse();

            b.SequenceEqual(a).ShouldBeTrue();

            await a.ToAsyncEnumerable().SequenceEqualsAsync(b.ToAsyncEnumerable()).ShouldBeTrueAsync();
        }

        [TestCase]
        public async Task SequenceEqualsThrowsArgumentNullExceptionWhenComparerIsNull()
        {
            var a = new Guid[0].ToAsyncEnumerable();
            var b = new Guid[0].ToAsyncEnumerable();

            a.ShouldNotBeNull();
            b.ShouldNotBeNull();

            IEqualityComparer<Guid> comparer = null;

            comparer.ShouldBeNull();

            await b.SequenceEqualsAsync(a, comparer).ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task SequenceEqualsThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> a = Integer.RandomSequence().ToAsyncEnumerable(), b = null;

            a.ShouldNotBeNull();

            b.ShouldBeNull();

            await b.SequenceEqualsAsync(a).ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task SequenceEqualsThrowsArgumentNullExceptionWhenTargetSequenceIsNull()
        {
            IAsyncEnumerable<int> a = null, b = Integer.RandomSequence().ToAsyncEnumerable();

            a.ShouldBeNull();

            b.ShouldNotBeNull();

            await b.SequenceEqualsAsync(a).ShouldThrowAsync<ArgumentNullException>();
        }
    }
}