using Bogosoft.Testing.Objects;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    [TestFixture, Category("Unit")]
    public class ZipMethodTests
    {
        IEnumerable<Guid> GetRandomGuids(int size)
        {
            for (var i = 0; i < size; i++)
            {
                yield return Guid.NewGuid();
            }
        }

        string Merge(int integer, Guid guid)
        {
            return $"{integer}, {guid}";
        }

        Task<string> MergeAsync(int integer, Guid guid, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return Task.FromResult(Merge(integer, guid));
        }

        [TestCase]
        public async Task ZipMergesTwoSequenceIntoANewSequence()
        {
            var size = 32;

            var a = Integer.RandomSequence(size).ToArray();
            var b = GetRandomGuids(size).ToArray();

            await using var x = a.ToAsyncEnumerable().GetAsyncEnumerator();
            await using var y = b.ToAsyncEnumerable().GetAsyncEnumerator();
            await using var z = a.ToAsyncEnumerable().ZipAsync(b.ToAsyncEnumerable(), Merge).GetAsyncEnumerator();

            while (await z.MoveNextAsync())
            {
                await x.MoveNextAsync().ShouldBeTrueAsync();
                await y.MoveNextAsync().ShouldBeTrueAsync();

                z.Current.ShouldBe(Merge(x.Current, y.Current));
            }

            await x.MoveNextAsync().ShouldBeFalseAsync();
            await y.MoveNextAsync().ShouldBeFalseAsync();
        }

        [TestCase]
        public void ZipThrowsArgumentNullExceptionWhenGivenSequenceIsNull()
        {
            IAsyncEnumerable<int> source = Integer.RandomSequence(56).ToAsyncEnumerable();

            source.ShouldNotBeNull();

            IAsyncEnumerable<Guid> given = null;

            given.ShouldBeNull();

            source.ZipAsync(given, Merge)
                  .ConsumeAsync()
                  .ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void ZipThrowsArgumentNullExceptionWhenSelectorIsNull()
        {
            var a = new object[0].ToAsyncEnumerable();
            var b = new string[0].ToAsyncEnumerable();

            a.ShouldNotBeNull();
            b.ShouldNotBeNull();

            Func<object, string, int> selector = null;

            selector.ShouldBeNull();

            a.ZipAsync(b, selector)
             .ConsumeAsync()
             .ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void ZipThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            var given = GetRandomGuids(16).ToAsyncEnumerable();

            given.ShouldNotBeNull();

            source.ZipAsync(given, Merge)
                  .ConsumeAsync()
                  .ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public async Task ZipWithAsyncSelectorMergesTwoSequencesIntoANewSequence()
        {
            var size = 32;

            var a = Integer.RandomSequence(size).ToArray();
            var b = GetRandomGuids(size).ToArray();

            await using var x = a.ToAsyncEnumerable().GetAsyncEnumerator();
            await using var y = b.ToAsyncEnumerable().GetAsyncEnumerator();
            await using var z = a.ToAsyncEnumerable().ZipAsync(b.ToAsyncEnumerable(), MergeAsync).GetAsyncEnumerator();

            while (await z.MoveNextAsync())
            {
                await x.MoveNextAsync().ShouldBeTrueAsync();
                await y.MoveNextAsync().ShouldBeTrueAsync();

                z.Current.ShouldBe(await MergeAsync(x.Current, y.Current, CancellationToken.None));
            }

            await x.MoveNextAsync().ShouldBeFalseAsync();
            await y.MoveNextAsync().ShouldBeFalseAsync();
        }

        [TestCase]
        public void ZipWithAsyncSelectorThrowsArgumentNullExceptionWhenGivenSequenceIsNull()
        {
            IAsyncEnumerable<int> source = Integer.RandomSequence(56).ToAsyncEnumerable();

            source.ShouldNotBeNull();

            IAsyncEnumerable<Guid> given = null;

            given.ShouldBeNull();

            source.ZipAsync(given, MergeAsync)
                  .ConsumeAsync()
                  .ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void ZipWithAsyncSelectorThrowsArgumentNullExceptionWhenSelectorIsNull()
        {
            var a = new object[0].ToAsyncEnumerable();
            var b = new string[0].ToAsyncEnumerable();

            a.ShouldNotBeNull();
            b.ShouldNotBeNull();

            Func<object, string, Task<int>> selector = null;

            a.ZipAsync(b, selector)
             .ConsumeAsync()
             .ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void ZipWithAsyncSelectorThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            var given = GetRandomGuids(16).ToAsyncEnumerable();

            given.ShouldNotBeNull();

            source.ZipAsync(given, MergeAsync)
                  .ConsumeAsync()
                  .ShouldThrow<ArgumentNullException>();
        }
    }
}