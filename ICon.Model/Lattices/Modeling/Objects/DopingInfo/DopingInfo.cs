using System;
using System.Collections.Generic;
using System.Text;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    /// Represents an actual doping that consists of an abstract doping description and a concentration value
    /// </summary>
    public readonly struct DopingInfo
    {
        /// <summary>
        /// The concentration of the doping particle
        /// </summary>
        public double Concentration { get; }

        /// <summary>
        /// The abstract description of the doping
        /// </summary>
        public IDoping AbstractDoping { get; }

        /// <summary>
        /// Create new doping info from concentration value and doping instruction
        /// </summary>
        /// <param name="concentration"></param>
        /// <param name="abstractDoping"></param>
        public DopingInfo(double concentration, IDoping abstractDoping) : this()
        {
            Concentration = concentration;
            AbstractDoping = abstractDoping;
        }
    }
}
