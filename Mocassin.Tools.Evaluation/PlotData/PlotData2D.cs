using System.Collections;
using System.Collections.Generic;

namespace Mocassin.Tools.Evaluation.PlotData
{
    /// <summary>
    ///     Represents a 2D set of plot data with X and Y errors
    /// </summary>
    /// <typeparam name="TX"></typeparam>
    /// <typeparam name="TY"></typeparam>
    public class PlotData2D<TX, TY> : IEnumerable<(TX X, TX ErrorX, TY Y, TY ErrorY)>
    {
        /// <summary>
        ///     The list of plot data values
        /// </summary>
        private List<(TX X, TX ErrorX, TY Y, TY ErrorY)> Values { get; }

        /// <summary>
        ///     Creates new <see cref="PlotData2D{TX,TY}" /> with initial capacity
        /// </summary>
        /// <param name="capacity"></param>
        public PlotData2D(int capacity = 0)
        {
            Values = new List<(TX, TX, TY, TY)>(capacity);
        }

        /// <inheritdoc />
        public IEnumerator<(TX X, TX ErrorX, TY Y, TY ErrorY)> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Values).GetEnumerator();
        }

        /// <summary>
        ///     Adds a plot point to the data
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="errorX"></param>
        /// <param name="errorY"></param>
        public void AddPoint(TX x, TY y, TX errorX = default, TY errorY = default)
        {
            Values.Add((x, errorX, y, errorY));
        }
    }
}