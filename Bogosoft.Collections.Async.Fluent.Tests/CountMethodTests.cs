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
    public class CountMethodTests
    {
        [TestCase]
        public async Task CountReturnsTheNumberOfElementsInASequence()
        {
            var source = Integer.RandomSequence().ToArray();

            await source.ToAsyncEnumerable().CountAsync().ShouldBeAsync(source.Length);
        }

        [TestCase]
        public async Task CountThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            await source.CountAsync().ShouldThrowAsync<long, ArgumentNullException>();
        }

        [TestCase]
        public void CountWithPredicateThrowsArgumentNullExceptionWhenPredicateIsNull()
        {
            var source = Integer.RandomSequence().ToAsyncEnumerable();

            source.ShouldNotBeNull();

            Func<int, bool> predicate = null;

            predicate.ShouldBeNull();

            Action test = () => source.CountAsync(predicate);

            test.ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void CountWithPredicateThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            Action test = () => source.CountAsync(Integer.Odd);

            test.ShouldThrow<ArgumentNullException>();
        }
    }
}