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
    public class FirstOrDefaultMethodTests
    {
        [TestCase]
        public async Task FirstOrDefaultReturnsDefaultValueFromEmtpySequence()
        {
            var source = new Guid[0];

            source.ShouldBeEmpty();

            await source.ToAsyncEnumerable().FirstOrDefaultAsync().ShouldBeAsync(default(Guid));
        }

        [TestCase]
        public async Task FirstOrDefaultReturnsFirstItemFromANonEmtpySequence()
        {
            var source = Integer.RandomSequence(4).ToArray();

            source.Length.ShouldBeGreaterThan(0);

            await source.ToAsyncEnumerable().FirstOrDefaultAsync().ShouldBeAsync(source[0]);
        }

        [TestCase]
        public async Task FirstOrDefaultThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            await source.FirstOrDefaultAsync().ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task FirstOrDefaultWithPredicateReturnsDefaultValueFromNonEmptyNonMatchingSequence()
        {
            int[] source;

            do
            {
                source = Integer.RandomSequence().Where(Integer.Even).ToArray();

            } while (source.Length == 0);

            source.Any(Integer.Odd).ShouldBeFalse();

            await source.ToAsyncEnumerable().FirstOrDefaultAsync(Integer.Odd).ShouldBeAsync(default(int));
        }

        [TestCase]
        public async Task FirstOrDefaultWithPredicateReturnsFirstMatchingElementFromSequence()
        {
            int[] source;

            do
            {
                source = Integer.RandomSequence().ToArray();

            } while (!source.Any(Integer.Even));

            var actual = source.First(Integer.Even);

            await source.ToAsyncEnumerable().FirstOrDefaultAsync(Integer.Even).ShouldBeAsync(actual);
        }

        [TestCase]
        public void FirstOrDefaultWithPredicateThrowsArgumentNullExceptionWhenPredicateIsNull()
        {
            var source = Integer.RandomSequence().ToArray();

            source.ShouldNotBeNull();

            Func<int, bool> predicate = null;

            predicate.ShouldBeNull();

            Action test = () => source.FirstOrDefault(predicate);

            test.ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void FirstOrDefaultWithPredicateThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            source.FirstOrDefaultAsync(Integer.Odd).ShouldThrow<ArgumentNullException>();
        }
    }
}