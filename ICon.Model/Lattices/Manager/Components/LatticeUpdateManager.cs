using Mocassin.Model.Energies.Handler;
using Mocassin.Model.Transitions;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Basic update manager for the lattice module that handles pushed information on data changes in required modules
    /// </summary>
    internal class LatticeUpdateManager : ModelUpdateManager<LatticeModelData, LatticeEventManager>, ILatticeUpdatePort
    {
        /// <summary>
        /// Create new Lattice update manager for the provided data object, event manager and project services
        /// </summary>
        /// <param name="modelData"></param>
        /// <param name="eventManager"></param>
        /// <param name="modelProject"></param>
        public LatticeUpdateManager(LatticeModelData modelData, LatticeEventManager eventManager, IModelProject modelProject)
            : base(modelData, eventManager, modelProject)
        {

        }
    }
}
