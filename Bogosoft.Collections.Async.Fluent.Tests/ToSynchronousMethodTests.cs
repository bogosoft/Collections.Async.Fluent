using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class ToSynchronousMethodTests
    {
        [TestCase]
        public void AttemptingToConvertNullAsynchronousSequenceToSynchronousThrowsArgumentNullException()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            Action test = () => source.ToSynchronous().Consume();

            test.ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void AsynchronousSequenceCanBeConvertedToSynchronousSequence()
        {
            int[] ints = { 0, 1, 2, 3, 4 };

            var a = ints.ToAsyncEnumerable();
            var b = a.ToSynchronous();

            b.ShouldBeAssignableTo<IEnumerable<int>>();

            b.Sum().ShouldBe(ints.Sum());
        }
    }
}