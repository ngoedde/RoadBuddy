namespace RB.Game.Client.ResourceLoader;

public interface ILoader<TResult> where TResult : ILoadResult
{
    /// <summary>
    ///     Loads the given path and returns the result.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    protected bool TryLoad(string path, out TResult result);
}