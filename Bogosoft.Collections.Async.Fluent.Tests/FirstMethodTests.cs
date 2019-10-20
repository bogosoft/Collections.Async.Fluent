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
    public class FirstMethodTests
    {
        [TestCase]
        public async Task FirstReturnsTheFirstItemFromANonEmptySequence()
        {
            var ints = Enumerable.Range(0, 15).ToArray();

            var source = ints.ToAsyncEnumerable();

            await source.FirstAsync().ShouldBeAsync(ints[0]);
        }

        [TestCase]
        public async Task FirstThrowsArgumentNullExceptionWhenSourceSequenceIsEmpty()
        {
            IAsyncEnumerable<Guid> source = null;

            source.ShouldBeNull();

            await source.FirstAsync().ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task FirstThrowsInvalidOperationExceptionWhenSourceSequenceIsEmpty()
        {
            var source = new string[0].ToAsyncEnumerable();

            Exception exception = null;

            try
            {
                await source.FirstAsync();
            }
            catch (Exception e)
            {
                exception = e;
            }

            exception.ShouldNotBeNull();

            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [TestCase]
        public void FirstWithPredicateThrowsArgumentNullExceptionWhenPredicateIsNull()
        {
            var source = Integer.RandomSequence().ToAsyncEnumerable();

            source.ShouldNotBeNull();

            Func<int, bool> predicate = null;

            predicate.ShouldBeNull();

            source.FirstAsync(predicate).ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void FirstWithPredicateThrownsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            source.FirstAsync(Integer.Even).ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public async Task FirstWithPredicateThrowsInvalidOperationExceptionWhenSourceSequenceIsEmpty()
        {
            var source = new int[0];

            source.ShouldBeEmpty();

            Exception exception = null;

            try
            {
                await source.ToAsyncEnumerable().FirstAsync(Integer.Odd);
            }
            catch (Exception e)
            {
                exception = e;
            }

            exception.ShouldNotBeNull();

            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [TestCase]
        public async Task FirstWithPredicateThrowsInvalidOperationExceptionWhenNoElementsMatchPredicate()
        {
            Exception exception = null;

            try
            {
                int[] ints;

                do
                {
                    ints = Integer.RandomSequence(64).Where(Integer.Even).ToArray();

                } while (ints.Length == 0);

                await ints.ToAsyncEnumerable().FirstAsync(Integer.Odd);
            }
            catch (Exception e)
            {
                exception = e;
            }

            exception.ShouldNotBeNull();

            exception.ShouldBeOfType<InvalidOperationException>();
        }
    }
}