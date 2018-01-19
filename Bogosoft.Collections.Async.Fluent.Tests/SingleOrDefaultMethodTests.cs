using Bogosoft.Testing.Objects;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class SingleOrDefaultMethodTests
    {
        [TestCase]
        public async Task SingleOrDefaultReturnsDefaultValueFromEmptySequence()
        {
            var source = new Guid[0];

            source.ShouldBeEmpty();

            await source.ToAsyncEnumerable().SingleOrDefaultAsync().ShouldBeAsync(default(Guid));
        }

        [TestCase]
        public async Task SingleOrDefaultReturnsSoleValueFromSequence()
        {
            var source = new Guid[] { Guid.NewGuid() };

            source.Length.ShouldBe(1);

            await source.ToAsyncEnumerable().SingleOrDefaultAsync().ShouldBeAsync(source[0]);
        }

        [TestCase]
        public async Task SingleOrDefaultThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<DateTimeOffset> source = null;

            source.ShouldBeNull();

            await source.SingleOrDefaultAsync().ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task SingleOrDefaultThrowsInvalidOperationExceptionWhenSequenceContainsMoreThanOneElement()
        {
            var source = Enumerable.Range(0, 2).ToArray();

            source.Length.ShouldBeGreaterThan(1);

            await source.ToAsyncEnumerable().SingleOrDefaultAsync().ShouldThrowAsync<InvalidOperationException>();
        }

        [TestCase]
        public async Task SingleOrDefaultWithPredicateReturnsDefaultValueFromSequenceWithNoMatchingElements()
        {
            int[] source;

            do
            {
                source = Integer.RandomSequence(256).Where(Integer.Even).ToArray();

            } while (source.Length == 0);

            source.Count(Integer.Odd).ShouldBe(0);

            await source.ToAsyncEnumerable().SingleOrDefaultAsync(Integer.Odd).ShouldBeAsync(default(int));
        }

        [TestCase]
        public async Task SingleOrDefaultWithPredicateReturnsSoleMatchingValueFromSequence()
        {
            var target = 10;

            Func<int, bool> predicate = x => x == target;

            var source = Enumerable.Range(0, 16).ToArray();

            source.Count(predicate).ShouldBe(1);

            await source.ToAsyncEnumerable().SingleOrDefaultAsync(predicate).ShouldBeAsync(target);
        }

        [TestCase]
        public void SingleOrDefaultWithPredicateThrowsArgumentNullExceptionWhenPredicateIsNull()
        {
            var source = Integer.RandomSequence().ToArray();

            source.ShouldNotBeNull();

            Func<int, bool> predicate = null;

            predicate.ShouldBeNull();

            Action test = () => source.SingleOrDefault(predicate);

            test.ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void SingleOrDefaultWithPredicateThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            Action test = () => source.SingleOrDefaultAsync(Integer.Even);

            test.ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public async Task SingleOrDefaultWithPredicateThrowsInvalidOperationExceptionWhenSequenceContainsMoreThanOneMatch()
        {
            int[] source;

            do
            {
                source = Integer.RandomSequence(64).ToArray();

            } while (source.Count(Integer.Odd) < 2);

            await source.ToAsyncEnumerable()
                        .SingleOrDefaultAsync(Integer.Odd)
                        .ShouldThrowAsync<InvalidOperationException>();
        }
    }
}