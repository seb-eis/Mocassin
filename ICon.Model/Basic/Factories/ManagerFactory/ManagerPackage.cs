using Mocassin.Model.Energies;
using Mocassin.Model.Lattices;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Simulations;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Manager package class that bundles a set of managers
    /// </summary>
    public class ManagerPackage
    {
        /// <summary>
        ///     Json input report by the automated data input system
        /// </summary>
        public string InputReportJson { get; set; }

        /// <summary>
        ///     Shared project services
        /// </summary>
        public IModelProject ModelProject { get; set; }

        /// <summary>
        ///     The particle manager
        /// </summary>
        public IParticleManager ParticleManager { get; set; }

        /// <summary>
        ///     The structure manager
        /// </summary>
        public IStructureManager StructureManager { get; set; }

        /// <summary>
        ///     The transition manager
        /// </summary>
        public ITransitionManager TransitionManager { get; set; }

        /// <summary>
        ///     The energy manager
        /// </summary>
        public IEnergyManager EnergyManager { get; set; }

        /// <summary>
        ///     The lattice manager
        /// </summary>
        public ILatticeManager LatticeManager { get; set; }

        /// <summary>
        ///     The simulation manager
        /// </summary>
        public ISimulationManager SimulationManager { get; set; }
    }
}