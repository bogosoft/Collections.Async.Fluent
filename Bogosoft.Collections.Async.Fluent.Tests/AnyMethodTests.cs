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
    public class AnyMethodTests
    {
        [TestCase]
        public async Task AnyReturnsFalseForEmptySequence()
        {
            var source = new int[0].ToAsyncEnumerable();

            await source.AnyAsync().ShouldBeFalseAsync();
        }

        [TestCase]
        public async Task AnyReturnsTrueForNonEmptySequence()
        {
            var source = Integer.RandomSequence(16).ToAsyncEnumerable();

            await source.AnyAsync().ShouldBeTrueAsync();
        }

        [TestCase]
        public async Task AnyWithPredicateReturnsFalseForNonEmptySequenceWithNoMatchingElements()
        {
            var source = new int[] { 1, 3, 5 };

            source.Any(Integer.Even).ShouldBeFalse();

            await source.ToAsyncEnumerable().AnyAsync(Integer.Even).ShouldBeFalseAsync();
        }

        [TestCase]
        public async Task AnyWithPredicateReturnsTrueForNonEmptySequenceWithMatchingElements()
        {
            var source = new int[] { 0, 2, 4 };

            source.Any(Integer.Even).ShouldBeTrue();

            await source.ToAsyncEnumerable().AnyAsync(Integer.Even).ShouldBeTrueAsync();
        }

        [TestCase]
        public void AnyThrowsArgumentNullExceptionWhenPredicateIsNull()
        {
            var source = Integer.RandomSequence(16).ToAsyncEnumerable();

            Action test = () => source.AnyAsync(null);

            test.ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void AnyThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            Action test = () => (null as IAsyncEnumerable<int>).AnyAsync(Integer.Even);

            test.ShouldThrow<ArgumentNullException>();
        }
    }
}