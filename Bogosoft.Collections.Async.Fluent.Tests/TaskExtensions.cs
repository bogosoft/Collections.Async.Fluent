using Shouldly;
using System;
using System.Threading.Tasks;

namespace Bogosoft.Collections.Async.Fluent.Tests
{
    static class TaskExtensions
    {
        internal static async Task ShouldBeAsync<T>(this Task<T> actual, T expected)
        {
            expected.ShouldBe(await actual.ConfigureAwait(false));
        }

        internal static async Task ShouldBeAsync<T>(this Task<T> actual, Task<T> expected)
        {
            (await expected.ConfigureAwait(false)).ShouldBe(await actual.ConfigureAwait(false));
        }

        internal static async Task ShouldBeFalseAsync(this Task<bool> task)
        {
            (await task.ConfigureAwait(false)).ShouldBeFalse();
        }

        internal static async Task ShouldBeTrueAsync(this Task<bool> task)
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
    }
}