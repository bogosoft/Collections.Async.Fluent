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
    public class ToArrayMethodTests
    {
        [TestCase]
        public async Task AttemptingToConvertNullSequenceToArrayThrowsArgumentNullException()
        {
            IAsyncEnumerable<DateTime> source = null;

            await source.ToArrayAsync().ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task CanConvertAsyncSequenceToArray()
        {
            var a = Integer.RandomSequence(64, 2048).ToList();
            var b = a.ToAsyncEnumerable();
            var c = await b.ToArrayAsync();

            c.Length.ShouldBe(a.Count);

            for (var i = 0; i < a.Count; i++)
            {
                c[i].ShouldBe(a[i]);
            }
        }
    }
}