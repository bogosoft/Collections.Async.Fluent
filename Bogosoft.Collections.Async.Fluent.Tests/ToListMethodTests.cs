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
    public class ToListMethodTests
    {
        [TestCase]
        public async Task AttemptingToCreateAListFromANullSequenceThrowsArgumentNullException()
        {
            IAsyncEnumerable<short> source = null;

            await source.ToListAsync().ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task CanCreateListFromAsyncSequence()
        {
            static bool Equals(CelestialBody x, CelestialBody y)
            {
                return x.Mass == y.Mass
                    && x.Name == y.Name
                    && x.Orbit.DistanceToPrimary == y.Orbit.DistanceToPrimary;
            }

            var a = CelestialBody.All.ToList();
            var b = await CelestialBody.All.ToAsyncEnumerable().ToListAsync();

            b.Count.ShouldBe(a.Count);

            for (var i = 0; i < a.Count; i++)
            {
                Equals(a[i], b[i]).ShouldBeTrue();
            }
        }
    }
}