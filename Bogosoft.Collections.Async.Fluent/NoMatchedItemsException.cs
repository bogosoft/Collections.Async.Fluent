namespace Bogosoft.Collections.Async.Fluent
{
    /// <summary>
    /// Indicates that an assumption about a non-empty sequence containing
    /// elements that match a specified condition was incorrect.
    /// </summary>
    public class NoMatchedItemsException : EnumerableAsyncException
    {
    }
}