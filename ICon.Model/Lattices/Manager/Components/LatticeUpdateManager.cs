using ICon.Model.Basic;
using ICon.Model.ProjectServices;
using ICon.Model.Energies.Handler;
using ICon.Model.Transitions;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Basic update manager for the lattice module that handles pushed information on data changes in required modules
    /// </summary>
    internal class LatticeUpdateManager : ModelUpdateManager<LatticeModelData, LatticeEventManager>, ILatticeUpdatePort
    {
        /// <summary>
        /// Create new Lattice update manager for the provided data object, event manager and project services
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="eventManager"></param>
        /// <param name="projectServices"></param>
        public LatticeUpdateManager(LatticeModelData baseData, LatticeEventManager eventManager, IProjectServices projectServices)
            : base(baseData, eventManager, projectServices)
        {

        }
    }
}
