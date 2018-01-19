using Bogosoft.Testing.Objects;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class WhereMethodTests
    {
        static bool IsPlanet(CelestialBody cb) => cb.Type == CelestialBodyType.Planet;

        [TestCase]
        public async Task WhereFiltersASequenceOfElements()
        {
            var all = CelestialBody.All;

            using (var a = all.ToAsyncEnumerable().Where(IsPlanet).GetEnumerator())
            using (var b = all.Where(IsPlanet).GetEnumerator())
            {
                while (await a.MoveNextAsync())
                {
                    b.MoveNext().ShouldBeTrue();

                    b.Current.Name.ShouldBe(a.Current.Name);
                }

                b.MoveNext().ShouldBeFalse();
            }
        }

        [TestCase]
        public void WhereThrowsArgumentNullExceptionWhenPredicateIsNull()
        {
            var source = CelestialBody.All.ToAsyncEnumerable();

            source.ShouldNotBeNull();

            Func<CelestialBody, bool> predicate = null;

            predicate.ShouldBeNull();

            Action test = () => source.Where(predicate);

            test.ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void WhereThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<CelestialBody> source = null;

            source.ShouldBeNull();

            Action test = () => source.Where(IsPlanet);

            test.ShouldThrow<ArgumentNullException>();
        }
    }
}