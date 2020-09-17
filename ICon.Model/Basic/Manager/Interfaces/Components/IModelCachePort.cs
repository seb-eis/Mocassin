namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     General base interface for all model data cache port interfaces
    /// </summary>
    public interface IModelCachePort : IModelDataPort
    {
        /// <summary>
        ///     Clears all cached data objects causing them to be recalculated on the next data access
        /// </summary>
        void ClearCachedData();
    }
}