using System.Collections.Generic;

namespace Mocassin.Framework.Constraints
{
    /// <summary>
    ///     Represents a double precision series of flp values ranging from a lower limit to an upper limit in internally
    ///     defined steps
    /// </summary>
    public interface IValueSeries : IEnumerable<double>
    {
        /// <summary>
        ///     The lower limit of the series. Values are greater or equal to this value
        /// </summary>
        double LowerLimit { get; }

        /// <summary>
        ///     The upper limit of the series. Values are lesser or equal to this value
        /// </summary>
        double UpperLimit { get; }

        /// <summary>
        ///     The number of values in the series
        /// </summary>
        /// <returns></returns>
        int GetValueCount();

        /// <summary>
        ///     Get an enumerbale seqeunce containing all values of the series
        /// </summary>
        /// <returns></returns>
        IEnumerable<double> GetValues();
    }
}