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
    public class ToHashSetMethodTests
    {
        [TestCase]
        public async Task AttemptingToCreateHashSetFromNullSequenceThrowsArgumentNullException()
        {
            IAsyncEnumerable<CelestialBody> source = null;

            await source.ToHashSetAsync().ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task CanCreateHashSetFromAsyncSequence()
        {
            int[] ints = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            var a = ints.ToHashSet();
            var b = await ints.ToAsyncEnumerable().ToHashSetAsync();

            b.ShouldBeOfType<HashSet<int>>();

            b.Count.ShouldBe(a.Count);

            for (var i = 0; i < a.Count; i++)
            {
                b.ElementAt(i).ShouldBe(a.ElementAt(i));
            }
        }

        [TestCase]
        public async Task HashSetCreatedFromAsyncSequenceDoesNotContainDuplicateValues()
        {
            var rng = new Random();
            var size = rng.Next(128, 256);
            var ints = new List<int>();

            for (var i = 0; i < size; i++)
            {
                var number = rng.Next();

                ints.Add(number);
                ints.Add(number);
            }

            ints.Count.ShouldBe(size * 2);

            ints.ToHashSet().Count.ShouldBe(size);

            (await ints.ToAsyncEnumerable().ToHashSetAsync()).Count.ShouldBe(size);
        }
    }
}