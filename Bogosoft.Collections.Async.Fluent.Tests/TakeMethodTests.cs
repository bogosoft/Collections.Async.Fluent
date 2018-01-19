using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class TakeMethodTests
    {
        [TestCase]
        public async Task TakeLimitsTheNumberOfElementsReturnedFromASequence()
        {
            ulong size = 8;

            var ints = Enumerable.Range(0, (int)size * 2);

            var sequence = ints.ToAsyncEnumerable();

            (await sequence.Take(size).ToArrayAsync()).SequenceEqual(ints.Take((int)size)).ShouldBeTrue();
        }

        [TestCase]
        public void TakeThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            Action test = () => source.Take(25);

            test.ShouldThrow<ArgumentNullException>();
        }
    }
}