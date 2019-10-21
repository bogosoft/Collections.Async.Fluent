using Bogosoft.Testing.Objects;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class ToDictionaryMethodTests
    {
        [TestCase]
        public async Task AttemptingToCreateADictionaryFromANullSequenceThrowsArgumentNullException()
        {
            IAsyncEnumerable<string> source = null;

            await source.ToDictionaryAsync(x => x, x => x).ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task CanCreateDictionaryFromAsyncSequence()
        {
            var map = await CelestialBody.All
                                         .ToAsyncEnumerable()
                                         .ToDictionaryAsync(x => x.Name, x => x.Mass);

            using var a = CelestialBody.All.GetEnumerator();
            using var b = map.GetEnumerator();

            while (a.MoveNext())
            {
                b.MoveNext().ShouldBeTrue();

                b.Current.Key.ShouldBe(a.Current.Name);
                b.Current.Value.ShouldBe(a.Current.Mass);
            }

            b.MoveNext().ShouldBeFalse();
        }
    }
}