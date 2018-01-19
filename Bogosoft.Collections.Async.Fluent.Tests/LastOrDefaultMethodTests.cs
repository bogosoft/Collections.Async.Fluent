using Bogosoft.Testing.Objects;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class LastOrDefaultMethodTests
    {
        [TestCase]
        public async Task LastOrDefaultReturnsDefaultValueFromEmptySequence()
        {
            var source = new DateTime[0];

            source.ShouldBeEmpty();

            await source.ToAsyncEnumerable().LastOrDefaultAsync().ShouldBeAsync(default(DateTime));
        }

        [TestCase]
        public async Task LastOrDefaultReturnsLastElementFromNonEmptySequence()
        {
            var source = Integer.RandomSequence().ToArray();

            source.ShouldNotBeEmpty();

            var last = source[source.Length - 1];

            await source.ToAsyncEnumerable().LastOrDefaultAsync().ShouldBeAsync(last);
        }

        [TestCase]
        public async Task LastOrDefaultThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<short> source = null;

            source.ShouldBeNull();

            await source.LastOrDefaultAsync().ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task LastOrDefaultWithPredicateReturnsDefaultValueFromSequenceWithNoMatchingElements()
        {
            int[] source;

            do
            {
                source = Integer.RandomSequence().Where(Integer.Even).ToArray();

            } while (source.Length == 0);

            await source.ToAsyncEnumerable().LastOrDefaultAsync(Integer.Odd).ShouldBeAsync(default(int));
        }

        [TestCase]
        public async Task LastOrDefaultWithPredicateReturnsLastMatchingElementFromNonEmptySequence()
        {
            int[] source;

            do
            {
                source = Integer.RandomSequence().ToArray();

            } while (!source.Any(Integer.Even));

            var last = source.Last(Integer.Even);

            await source.ToAsyncEnumerable().LastOrDefaultAsync(Integer.Even).ShouldBeAsync(last);
        }

        [TestCase]
        public void LastOrDefaultWithPredicateThrowsArgumentNullExceptionWhenPredicateIsNull()
        {
            var source = Integer.RandomSequence().ToAsyncEnumerable();

            source.ShouldNotBeNull();

            Func<int, bool> predicate = null;

            predicate.ShouldBeNull();

            Action test = () => source.LastOrDefaultAsync(predicate);

            test.ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void LastOrDefaultWithPredicateThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<byte> source = null;

            source.ShouldBeNull();

            Action test = () => source.LastOrDefaultAsync(x => x % 2 == 3);

            test.ShouldThrow<ArgumentNullException>();
        }
    }
}