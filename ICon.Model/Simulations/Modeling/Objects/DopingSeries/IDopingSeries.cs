using System.Collections.Generic;
using Mocassin.Framework.Constraints;
using Mocassin.Model.Lattices;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    ///     Represents a doping series providing an abstract doping information and a value series of actual concentrations
    /// </summary>
    public interface IDopingSeries : IEnumerable<double>
    {
        /// <summary>
        ///     The abstract doping information for the series
        /// </summary>
        IDoping Doping { get; }

        /// <summary>
        ///     The actual concentration values of the series
        /// </summary>
        IValueSeries ValueSeries { get; }
    }
}