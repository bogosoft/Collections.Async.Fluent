using Bogosoft.Testing.Objects;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Linq.Tests
{
    [TestFixture, Category("Unit")]
    public class CountedSequenceTests
    {
        //[TestCase]
        //public async Task CountedSequenceCountsNumberOfItemsTraversed()
        //{
        //    int size = 64;

        //    var sequence = new CountedSequence<int>(Integer.RandomSequence(size).ToTraversable());

        //    sequence.Counted.ShouldEqual(0);

        //    using (var cursor = await sequence.GetCursorAsync())
        //    {
        //        while(await cursor.MoveNextAsync())
        //        {
        //            // Do nothing
        //        }
        //    }

        //    sequence.Counted.ShouldEqual(size);
        //}
    }
}