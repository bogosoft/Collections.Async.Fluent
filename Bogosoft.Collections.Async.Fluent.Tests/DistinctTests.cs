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
    public class DistinctTests
    {
        class CelestialBodyTypeEqualityComparer : EqualityComparer<CelestialBody>
        {
            public override bool Equals(CelestialBody x, CelestialBody y) => x.Type == y.Type;

            public override int GetHashCode(CelestialBody cb) => cb.Type.GetHashCode();
        }

        [TestCase]
        public void CallingDistinctOnANonNullSequenceWithANullComparerThrowsArgumentNullException()
        {
            var source = new int[] { 1, 2, 3, 4, 5 }.ToAsyncEnumerable();

            source.ShouldNotBeNull();

            IEqualityComparer<int> comparer = null;

            Action test = () => source = source.Distinct(comparer);

            test.ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void CallingDistinctOnANullSequenceThrowsArgumentNullException()
        {
            IAsyncEnumerable<int> source = null;

            Action test = () => source = source.Distinct();

            test.ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public async Task CallingDistinctOnASequenceWithRepeatedValuesReturnsOnlyDistinctValues()
        {
            var source = new int[] { 1, 1, 2, 2, 2, 2, 3, 3, 3 };

            source.Length.ShouldBeGreaterThan(source.Distinct().Count());

            var actual = await source.ToAsyncEnumerable().Distinct().ToArrayAsync();

            actual.Length.ShouldBe(source.Distinct().Count());
        }

        [TestCase]
        public async Task CallingDistinctOnASequenceWithRepeatedValuesWithComparerReturnsOnlyDistinctValues()
        {
            var source = CelestialBody.All.ToArray();

            var comparer = new CelestialBodyTypeEqualityComparer();

            var expected = source.Distinct(comparer).ToArray();

            expected.Length.ShouldBeLessThan(source.Length);

            var actual = await source.ToAsyncEnumerable().Distinct(comparer).ToArrayAsync();

            actual.Length.ShouldBe(expected.Length);

            actual.SequenceEqual(expected).ShouldBeTrue();
        }
    }
}