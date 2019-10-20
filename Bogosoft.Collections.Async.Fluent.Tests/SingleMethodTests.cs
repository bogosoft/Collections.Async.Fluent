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
    public class SingleMethodTests
    {
        static bool IsEight(int value) => value == 8;

        [TestCase]
        public async Task SingleReturnsTheSoleItemInASequence()
        {
            var source = Enumerable.Range(0, 1).ToArray();

            source.Length.ShouldBe(1);

            await source.ToAsyncEnumerable().SingleAsync().ShouldBeAsync(source[0]);
        }

        [TestCase]
        public async Task SingleThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<sbyte> source = null;

            source.ShouldBeNull();

            await source.SingleAsync().ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task SingleThrowsInvalidOperationExceptionWhenSequenceContainsMoreThanOneItem()
        {
            Exception exception = null;

            try
            {
                await Enumerable.Range(0, 2).ToAsyncEnumerable().SingleAsync();
            }
            catch (Exception e)
            {
                exception = e;
            }

            exception.ShouldNotBeNull();

            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [TestCase]
        public async Task SingleThrowsInvalidOperationExceptionWhenSequenceIsEmpty()
        {
            Exception exception = null;

            try
            {
                var ignore = await Enumerable.Range(0, 0).ToAsyncEnumerable().SingleAsync();
            }
            catch (Exception e)
            {
                exception = e;
            }

            exception.ShouldNotBeNull();

            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [TestCase]
        public async Task SingleWithPredicateReturnsTheSoleItemInASequenceThatMatches()
        {
            var source = Enumerable.Range(0, 16).ToArray();

            source.Count(IsEight).ShouldBe(1);

            await source.ToAsyncEnumerable().SingleAsync(IsEight).ShouldBeAsync(8);
        }

        [TestCase]
        public void SingleWithPredicateThrowsArgumentNullExceptionWhenPredicateIsNull()
        {
            var source = new string[0].ToAsyncEnumerable();

            source.ShouldNotBeNull();

            Func<string, bool> predicate = null;

            predicate.ShouldBeNull();

            source.SingleAsync(predicate).ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void SingleWithPredicateThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            source.SingleAsync(Integer.Even).ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public async Task SingleWithPredicateThrowsInvalidOperationExceptionWhenSequenceContainsMoreThanOneMatch()
        {
            Exception exception = null;

            var ints = Enumerable.Range(0, 8);

            ints.Count(Integer.Even).ShouldBeGreaterThan(1);

            try
            {
                await ints.ToAsyncEnumerable().SingleAsync(Integer.Even);
            }
            catch (Exception e)
            {
                exception = e;
            }

            exception.ShouldNotBeNull();

            exception.ShouldBeOfType<InvalidOperationException>();
        }

        [TestCase]
        public async Task SingleWithPredicateThrowsInvalidOperationExceptionWhenSequenceContainsNoMatchingItems()
        {
            var size = 8;

            Exception exception = null;

            try
            {
                await Enumerable.Range(0, size)
                                            .ToAsyncEnumerable()
                                            .SingleAsync(x => x == size);
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