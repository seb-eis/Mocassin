using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// The mccs meta component that carries all encoded misc simulation data and meta information
    /// </summary>
    public class MccsMetaInfo : MccsComponent
    {
        /// <summary>
        /// The list of particles used in the simulation
        /// </summary>
        public List<ParticleEntity> Particles { get; set; }

        /// <summary>
        /// Normed component of the electric field vector in A direction
        /// </summary>
        public double EFieldComponentA { get; set; }

        /// <summary>
        /// Normed component of the electric field vector in B direction
        /// </summary>
        public double EFieldComponentB { get; set; }

        /// <summary>
        /// Normed component of the electric field vector in C direction
        /// </summary>
        public double EFieldComponentC { get; set; }
    }
}
