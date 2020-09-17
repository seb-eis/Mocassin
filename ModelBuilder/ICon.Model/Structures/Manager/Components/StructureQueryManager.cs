using Mocassin.Model.Basic;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Basic implementation of the structure query manager that handles safe data queries and service requests to the
    ///     structure manager from outside sources
    /// </summary>
    internal class StructureQueryManager :
        ModelQueryManager<StructureModelData, IStructureDataPort, StructureModelCache, IStructureCachePort>, IStructureQueryPort
    {
        /// <inheritdoc />
        public StructureQueryManager(StructureModelData modelData, StructureModelCache modelCache, AccessLockSource lockSource)
            : base(modelData, modelCache, lockSource)
        {
        }
    }
}