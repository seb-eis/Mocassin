using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Constraints;
using ICon.Model.Lattices;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Represents a doping series providing an abstract doping information and a value series of actual concentrations
    /// </summary>
    public interface IDopingSeries : IEnumerable<double>
    {
        /// <summary>
        /// The abstract doping information for the series
        /// </summary>
        IDoping Doping { get; }

        /// <summary>
        /// The actual concentration values of the series
        /// </summary>
        IValueSeries ValueSeries { get; }
    }
}
