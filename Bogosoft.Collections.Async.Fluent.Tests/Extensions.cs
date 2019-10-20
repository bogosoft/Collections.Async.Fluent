using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    static class Extensions
    {
        internal static async Task ConsumeAsync<T>(
            this IAsyncEnumerable<T> source,
            CancellationToken token = default
            )
        {
            await foreach (var x in source.WithCancellation(token))
            {
            }
        }

        internal static async Task ShouldBeAsync<T>(this Task<T> actual, T expected)
        {
            expected.ShouldBe(await actual.ConfigureAwait(false));
        }

        internal static async Task ShouldBeAsync<T>(this ValueTask<T> actual, T expected)
        {
            expected.ShouldBe(await actual.ConfigureAwait(false));
        }

        internal static async Task ShouldBeAsync<T>(this Task<T> actual, Task<T> expected)
        {
            (await expected.ConfigureAwait(false)).ShouldBe(await actual.ConfigureAwait(false));
        }

        internal static async ValueTask ShouldBeFalseAsync(this ValueTask<bool> task)
        {
            (await task.ConfigureAwait(false)).ShouldBeFalse();
        }

        internal static async ValueTask ShouldBeTrueAsync(this ValueTask<bool> task)
        {
            (await task.ConfigureAwait(false)).ShouldBeTrue();
        }

        internal static async Task ShouldThrowAsync<T>(this Task task) where T : Exception
        {
            Exception exception = null;

            try
            {
                await task.ConfigureAwait(false);
            }
            catch(Exception e)
            {
                exception = e;
            }

            exception.ShouldNotBeNull();

            exception.ShouldBeAssignableTo<T>();
        }

        internal static async Task ShouldThrowAsync<T>(this ValueTask task) where T : Exception
        {
            Exception exception = null;

            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                exception = e;
            }

            exception.ShouldNotBeNull();

            exception.ShouldBeAssignableTo<T>();
        }

        internal static async Task ShouldThrowAsync<TResult, TException>(this ValueTask<TResult> task)
            where TException : Exception
        {
            Exception exception = null;

            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                exception = e;
            }

            exception.ShouldNotBeNull();

            exception.ShouldBeAssignableTo<TException>();
        }
    }
}