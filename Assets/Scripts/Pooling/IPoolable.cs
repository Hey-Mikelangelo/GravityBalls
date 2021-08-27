public interface IPoolable
{
    /// <summary>
    /// Prepare the entire game object for being pooled
    /// </summary>
    void BeforeReturnToPool();
    /// <summary>
    /// Prepare the game object for spawn.
    /// </summary>
    void OnSpawnFromPool();
}
    