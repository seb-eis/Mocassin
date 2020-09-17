using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Data cache for the extended on-demand transition model data
    /// </summary>
    internal class TransitionModelCache : ModelDataCache<ITransitionCachePort>
    {
        /// <inheritdoc />
        public TransitionModelCache(IModelEventPort eventPort, IModelProject modelProject)
            : base(eventPort, modelProject)
        {
        }

        /// <inheritdoc />
        public override ITransitionCachePort AsReadOnly()
        {
            return CachePort ??= new TransitionCacheManager(this, ModelProject);
        }
    }
}