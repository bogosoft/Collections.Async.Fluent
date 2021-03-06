﻿using Bogosoft.Testing.Objects;
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
    public class SelectMethodTests
    {
        static string ToString(int value) => value.ToString();

        static Task<string> ToStringAsync(int value, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            return Task.FromResult(value.ToString());
        }

        [TestCase]
        public async Task SelectTransformsSequence()
        {
            var ints = Integer.RandomSequence(256).ToArray();

            var source = ints.ToAsyncEnumerable();

            await using var a = source.GetAsyncEnumerator();
            await using var b = source.SelectAsync(ToString).GetAsyncEnumerator();

            while (await a.MoveNextAsync())
            {
                await b.MoveNextAsync().ShouldBeTrueAsync();

                b.Current.ShouldBe(ToString(a.Current));
            }

            await b.MoveNextAsync().ShouldBeFalseAsync();
        }

        [TestCase]
        public async Task SelectTransformsSequenceWithAsyncSelector()
        {
            var ints = Integer.RandomSequence(256).ToArray();

            var source = ints.ToAsyncEnumerable();

            await using var a = source.GetAsyncEnumerator();
            await using var b = source.SelectAsync(ToStringAsync).GetAsyncEnumerator();

            while (await a.MoveNextAsync())
            {
                await b.MoveNextAsync().ShouldBeTrueAsync();

                b.Current.ShouldBe(await ToStringAsync(a.Current, CancellationToken.None));
            }

            await b.MoveNextAsync().ShouldBeFalseAsync();
        }

        [TestCase]
        public void SelectThrowsArgumentNullExceptionWhenSelectorIsNull()
        {
            var source = Integer.RandomSequence().ToArray();

            source.ShouldNotBeNull();

            Func<int, Guid> selector = null;

            selector.ShouldBeNull();

            Action test = () => source.Select(selector);

            test.ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void SelectThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            source.SelectAsync(ToString)
                  .ConsumeAsync()
                  .ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void SelectWithAsyncSelectorThrowsArgumentNullExceptionWhenSelectorIsNull()
        {
            var source = Integer.RandomSequence().ToAsyncEnumerable();

            source.ShouldNotBeNull();

            Func<int, CancellationToken, Task<string>> selector = null;

            selector.ShouldBeNull();

            source.SelectAsync(selector)
                  .ConsumeAsync()
                  .ShouldThrow<ArgumentNullException>();
        }

        [TestCase]
        public void SelectWithAsyncSelectorThrowsArgumentNullExceptionWhenSourceSequenceIsNull()
        {
            IAsyncEnumerable<int> source = null;

            source.ShouldBeNull();

            source.SelectAsync(ToStringAsync)
                  .ConsumeAsync()
                  .ShouldThrow<ArgumentNullException>();
        }
    }
}