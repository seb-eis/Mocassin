namespace Mocassin.Tools.Evaluation.PlotData
{
    /// <summary>
    ///     A generic XY data point with errors
    /// </summary>
    /// <typeparam name="TX"></typeparam>
    /// <typeparam name="TY"></typeparam>
    public class DataPoint<TX, TY>
    {
        /// <summary>
        ///     Get the X value
        /// </summary>
        public TX X { get; }

        /// <summary>
        ///     Get the X error
        /// </summary>
        public TX ErrorX { get; }

        /// <summary>
        ///     Get the Y value
        /// </summary>
        public TY Y { get; }

        /// <summary>
        ///     Get the Y error
        /// </summary>
        public TY ErrorY { get; }

        /// <summary>
        ///     Creates a new <see cref="DataPoint{TX,TY}" /> with values and errors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="errorX"></param>
        /// <param name="y"></param>
        /// <param name="errorY"></param>
        public DataPoint(TX x, TX errorX, TY y, TY errorY)
        {
            X = x;
            ErrorX = errorX;
            Y = y;
            ErrorY = errorY;
        }

        /// <summary>
        ///     Creates a new <see cref="DataPoint{TX,TY}" /> with values and default errors
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public DataPoint(TX x, TY y)
            : this(x, default, y, default)
        {
        }
    }
}