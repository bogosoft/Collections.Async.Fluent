using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class OfTypeMethodTests
    {
        static bool IsString(object value) => value is string;

        [TestCase]
        public async Task OfTypeReturnsSequenceOnlyOfGivenDerivedType()
        {
            var items = new object[]
            {
                10,
                "string",
                DateTime.Now,
                0x80,
                Guid.NewGuid().ToString(),
                89f,
                new KeyValuePair<int, string>(0, "zero"),
                Guid.NewGuid(),
                DateTimeOffset.Now.AddDays(1),
                2,
                798u
            };

            var actual = (int)await items.ToAsyncEnumerable().OfType<object, string>().CountAsync();

            var expected = items.Where(IsString).Count();

            expected.ShouldBe(actual);
        }

        [TestCase]
        public void OfTypeThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<object> source = null;

            source.ShouldBeNull();

            Action test = () => source.OfType<object, string>();

            test.ShouldThrow<ArgumentNullException>();
        }
    }
}