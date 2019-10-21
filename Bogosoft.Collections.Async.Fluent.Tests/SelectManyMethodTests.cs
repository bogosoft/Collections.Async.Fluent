using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class SelectManyMethodTests
    {
        [TestCase]
        public async Task AttemptingToSelectManyOfEitherTypeOfNullSequenceThrowsArgumentNullException()
        {
            IAsyncEnumerable<int> a = null;
            IEnumerable<int> b = null;

            a.ShouldBeNull();
            b.ShouldBeNull();

            static IEnumerable<T> Project<T>(T t)
            {
                yield break;
            }

#pragma warning disable CS1998
            static async IAsyncEnumerable<T> ProjectAsync<T>(T t)
#pragma warning restore CS1998
            {
                yield break;
            }

            await a.SelectManyAsync(ProjectAsync)
                   .ConsumeAsync()
                   .ShouldThrowAsync<ArgumentNullException>();

            await a.SelectManyAsync(Project)
                   .ConsumeAsync()
                   .ShouldThrowAsync<ArgumentNullException>();

            await b.SelectManyAsync(ProjectAsync)
                   .ConsumeAsync()
                   .ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task AttemptingToSelectManyWithANullSelectorThrowsArgumentNullException()
        {
            var a = new int[0];
            var b = a.ToAsyncEnumerable();

            Func<int, IAsyncEnumerable<int>> selector1 = null;
            Func<int, IEnumerable<int>> selector2 = null;

            selector1.ShouldBeNull();
            selector2.ShouldBeNull();

            a.ShouldNotBeNull();
            b.ShouldNotBeNull();

            await a.SelectManyAsync(selector1)
                   .ConsumeAsync()
                   .ShouldThrowAsync<ArgumentNullException>();

            await b.SelectManyAsync(selector1)
                   .ConsumeAsync()
                   .ShouldThrowAsync<ArgumentNullException>();

            await b.SelectManyAsync(selector2)
                   .ConsumeAsync()
                   .ShouldThrowAsync<ArgumentNullException>();
        }

        [TestCase]
        public async Task CanProjectAndFlattenSequenceOfEitherType()
        {
            var a = new string[] { "Hello", ", ", "World!" };
            var b = a.ToAsyncEnumerable();

            static IEnumerable<char> Decompose(string s) => s;

            static IAsyncEnumerable<char> DecomposeAsync(string s) => s.ToAsyncEnumerable();

            var x = await a.SelectManyAsync(DecomposeAsync).ToArrayAsync();
            var y = await b.SelectManyAsync(Decompose).ToArrayAsync();
            var z = await b.SelectManyAsync(DecomposeAsync).ToArrayAsync();

            string.Join("", x).ShouldBe("Hello, World!");
            string.Join("", y).ShouldBe("Hello, World!");
            string.Join("", z).ShouldBe("Hello, World!");
        }
    }
}