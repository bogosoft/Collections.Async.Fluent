using Bogosoft.Testing.Objects;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class LastMethodTests
    {
        [TestCase]
        public async Task LastReturnsLastItemInANonEmptySequence()
        {
            var source = Integer.RandomSequence().ToArray();

            source.ShouldNotBeEmpty();

            var last = source[source.Length - 1];

            await source.ToAsyncEnumerable().LastAsync().ShouldBeAsync(last);
        }

        [TestCase]
        public async Task LastThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<float> source = null;

            source.ShouldBeNull();

            await source.LastAsync().ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task LastThrowsInvalidOperationExceptionOnEmptySequence()
        {
            var source = new DateTime[0].ToAsyncEnumerable();

            Exception exception = null;

            try
            {
                await source.LastAsync();
            }
            catch (Exception e)
            {
                exception = e;
            }

            exception.ShouldNotBeNull();

            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [TestCase]
        public async Task LastWithPredicateReturnsLastMatchingElementFromNonEmptySequence()
        {
            int[] source;

            do
            {
                source = Integer.RandomSequence(16).ToArray();

            } while (!source.Any(Integer.Odd));

            var last = source.Last(Integer.Odd);

            await source.ToAsyncEnumerable().LastAsync(Integer.Odd).ShouldBeAsync(last);
        }

        [TestCase]
        public void LastWithPredicateThrowsArgumentNullExceptionWhenPredicateIsNull()
        {
            var source = Integer.RandomSequence().ToAsyncEnumerable();

            source.ShouldNotBeNull();

            Func<int, bool> predicate = null;

            predicate.ShouldBeNull();

            Action test = () => source.LastAsync(predicate);

            test.ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void LastWithPredicateThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            Action test = () => source.LastAsync(Integer.Even);

            test.ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public async Task LastWithPredicateThrowsInvalidOperationExceptionWhenSequenceContainsNoMatches()
        {
            int[] source;

            do
            {
                source = Integer.RandomSequence().Where(Integer.Even).ToArray();

            } while (source.Length == 0);

            source.Any(Integer.Odd).ShouldBeFalse();

            await source.ToAsyncEnumerable().LastAsync(Integer.Odd).ShouldThrowAsync<InvalidOperationException>();
        }
    }
}