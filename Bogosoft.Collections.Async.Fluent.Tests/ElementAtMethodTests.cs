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
    public class ElementAtMethodTests
    {
        [TestCase]
        public async Task AttemptingToObtainElementAtNegativeIndexThrowsArgumentOutOfRangeException()
        {
            var source = Integer.RandomSequence(16, 64).ToAsyncEnumerable();

            await source.ElementAtAsync(-1).ShouldThrowAsync<ArgumentOutOfRangeException>();
        }

        [TestCase]
        public async Task AttemptingToObtainElementFromNullSequenceThrowsArgumentNullException()
        {
            IAsyncEnumerable<char> source = null;

            await source.ElementAtAsync(0).ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task AttemptingToObtainElementWhenIndexEqualsSequenceLengthThrowsArgumentOutOfRangeException()
        {
            var ints = Integer.RandomSequence(64).ToArray();

            var source = ints.ToAsyncEnumerable();

            var index = ints.Length;

            index.ShouldBe(ints.Length);

            await source.ElementAtAsync(index).ShouldThrowAsync<ArgumentOutOfRangeException>();
        }

        [TestCase]
        public async Task AttemptingToObtainElementWhenIndexExceedsSequenceLengthThrowsArgumentOutOfRangeException()
        {
            var ints = Integer.RandomSequence(64).ToArray();

            var source = ints.ToAsyncEnumerable();

            var index = ints.Length + 1;

            index.ShouldBeGreaterThan(ints.Length);

            await source.ElementAtAsync(index).ShouldThrowAsync<ArgumentOutOfRangeException>();
        }

        [TestCase]
        public async Task CanObtainElementFromGivenIndexInAsyncSequence()
        {
            var ints = Integer.RandomSequence(256, -32768, 32767).ToArray();

            var source = ints.ToAsyncEnumerable();

            for (var i = 0; i < ints.Length; i++)
            {
                await source.ElementAtAsync(i).ShouldBeAsync(ints[i]);
            }
        }
    }
}