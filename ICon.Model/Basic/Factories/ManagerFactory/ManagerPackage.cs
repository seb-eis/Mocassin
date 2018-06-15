using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.ProjectServices;
using ICon.Model.Particles;
using ICon.Model.Structures;
using ICon.Model.Transitions;
using ICon.Model.Energies;
using ICon.Model.Lattices;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Manager apckage class that bundles a set of managers
    /// </summary>
    public class ManagerPackage
    {
        /// <summary>
        /// Shared project services
        /// </summary>
        public IProjectServices ProjectServices { get; set; }

        /// <summary>
        /// The particle manager
        /// </summary>
        public IParticleManager ParticleManager { get; set; }

        /// <summary>
        /// The structure manager
        /// </summary>
        public IStructureManager StructureManager { get; set; }

        /// <summary>
        /// The transition manager
        /// </summary>
        public ITransitionManager TransitionManager { get; set; }

        /// <summary>
        /// The energy manager
        /// </summary>
        public IEnergyManager EnergyManager { get; set; }

        /// <summary>
        /// The lattice manager
        /// </summary>
        public ILatticeManager LatticeManager { get; set; }
    }
}
