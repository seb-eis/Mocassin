namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     General interface for all 128 bit encoded linear 4D information
    /// </summary>
    public interface ILinearVector4D
    {
        /// <summary>
        ///     The 32 bit integer coordinate tuple
        /// </summary>
        Coordinates<int, int, int, int> Coordinates { get; }
    }

    /// <summary>
    ///     General interface for all 128 bit encoded linear 4D information that support copy factories
    /// </summary>
    /// <typeparam name="TVector"></typeparam>
    public interface ILinearVector4D<out TVector> : ILinearVector4D where TVector : struct, ILinearVector4D<TVector>
    {
        /// <summary>
        ///     Factory function to create new 4D vectors of this type
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        TVector CreateNew(int a, int b, int c, int p);
    }
}