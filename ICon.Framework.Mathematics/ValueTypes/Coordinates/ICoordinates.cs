namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     Defines a coordinate object that holds coordinate information of specific dimension
    /// </summary>
    public interface ICoordinates
    {
        /// <summary>
        ///     Get the dimension of the coordinates
        /// </summary>
        int Dimension { get; }
    }
}