namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Represents an array like type that allows byte set/get operations by 4 indices
    /// </summary>
    public interface IByteArray4D
    {
        /// <summary>
        ///     Access the data by index (a,b,c,d)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        byte this[int a, int b, int c, int d] { get; set; }

        /// <summary>
        ///     Get the length in the specified dimension
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        int GetLength(int dimension);

        /// <summary>
        ///     Get the contents as an actual byte[,,,]
        /// </summary>
        /// <returns></returns>
        byte[,,,] AsArray();
    }
}