using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mocassin.Tools.Evaluation.PlotData
{
    /// <summary>
    ///     Represents a 2D set of plot data with X and Y errors
    /// </summary>
    /// <typeparam name="TX"></typeparam>
    /// <typeparam name="TY"></typeparam>
    public class PlotPoints<TX, TY> : IEnumerable<DataPoint<TX, TY>>
    {
        /// <summary>
        ///     The list of plot data values
        /// </summary>
        private List<DataPoint<TX, TY>> Values { get; }

        /// <summary>
        ///     Creates new <see cref="PlotPoints{TX,TY}" /> with initial capacity
        /// </summary>
        /// <param name="capacity"></param>
        public PlotPoints(int capacity = 0)
        {
            Values = new List<DataPoint<TX,TY>>(capacity);
        }

        /// <summary>
        ///     Creates new <see cref="PlotPoints{TX,TY}"/> from a sequence of <see cref="DataPoint{TX,TY}"/>
        /// </summary>
        /// <param name="points"></param>
        public PlotPoints(IEnumerable<DataPoint<TX, TY>> points)
        {
            Values = points.ToList();
        }

        /// <inheritdoc />
        public IEnumerator<DataPoint<TX, TY>> GetEnumerator()
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
            Values.Add(new DataPoint<TX, TY>(x, errorX, y, errorY));
        }

        /// <summary>
        ///     Writes the data to a file with the provided format
        /// </summary>
        /// <param name="path"></param>
        /// <param name="format"></param>
        public void WriteToFile(string path, string format = "{}\t{}")
        {
            using var stream = File.CreateText(path);
            foreach (var point in Values)
            {
                stream.WriteLine(format, point.X, point.Y);
            }
        }
    }
}