namespace RB.Game.Client.ResourceLoader;

public interface ILoader<TResult, TExpectedVal> where TResult : LoaderResult<TExpectedVal>
{
    protected bool TryLoad(string path, out TResult result);
}