using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Cache for extended structure data that stores 'on-demand' calculated dependent data for faster access until the
    ///     data is no longer valid
    /// </summary>
    internal class StructureModelCache : ModelDataCache<IStructureCachePort>
    {
        /// <inheritdoc />
        public StructureModelCache(IModelEventPort eventPort, IModelProject modelProject)
            : base(eventPort, modelProject)
        {
        }

        /// <inheritdoc />
        public override IStructureCachePort AsReadOnly()
        {
            return CachePort ?? (CachePort = new StructureCacheManager(this, ModelProject));
        }
    }
}