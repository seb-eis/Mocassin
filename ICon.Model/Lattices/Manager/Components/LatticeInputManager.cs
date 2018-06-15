using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Basic implementation of the lattice input manager that handles validated adding, removal and replacement of lattice base data by an outside source
    /// </summary>
    internal class LatticeInputManager : ModelInputManager<LatticeModelData, ILatticeDataPort, LatticeEventManager>, ILatticeInputPort
    {
        /// <summary>
        /// Create new lattice input manager from data object, event manager and project services
        /// </summary>
        /// <param name="data"></param>
        /// <param name="manager"></param>
        /// <param name="services"></param>
        public LatticeInputManager(LatticeModelData data, LatticeEventManager manager, IProjectServices services) : base(data, manager, services)
        {

        }

        /// <summary>
        /// Get the lattice conflict resolver provider that provides conflicts resolvers for internal data conflicts in this manager
        /// </summary>
        /// <returns></returns>
        protected override IDataConflictHandlerProvider<LatticeModelData> MakeConflictHandlerProvider()
        {
            return new LatticeDataConflictResolverProvider(ProjectServices);
        }

        /// <summary>
        /// Tries to clean deprecated data by removing deprecated model objects and reindexing the model object lists. Distributes affiliated eventy on operation success
        /// </summary>
        /// <returns></returns>
        [OperationMethod(DataOperationType.ObjectCleaning)]
        protected override IOperationReport TryCleanDeprecatedData()
        {
            return DefaultCleanDeprecatedData();
        }
    }
}
