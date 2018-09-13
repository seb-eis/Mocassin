using ICon.Model.Energies;
using ICon.Model.Particles;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Describes a single pair energy model with energy table and reference geometry information
    /// </summary>
    public interface IPairEnergyModel : IModelComponent
    {
        /// <summary>
        /// Boolean flag that indictes if the interaction behaves asymetrically
        /// </summary>
        bool IsAsymmetric { get; }

        /// <summary>
        /// The pair interaction the model is based upon
        /// </summary>
        IPairInteraction PairInteraction { get; set; }

        /// <summary>
        /// The enrgy dictionary that assigns each pair of particles an energy value
        /// </summary>
        IDictionary<(IParticle, IParticle), double> EnergyDictionary { get; set; }

        /// <summary>
        /// The pair energy table that assignes each particle index pair an energy value
        /// </summary>
        double[,] EnergyTable { get; set; }
    }
}
