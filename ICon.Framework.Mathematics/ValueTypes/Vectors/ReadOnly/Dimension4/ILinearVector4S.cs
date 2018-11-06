namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    ///     General interface for all 64 bit encoded linear 4D information
    /// </summary>
    public interface ILinearVector4S
    {
        /// <summary>
        ///     The 32 bit integer coordinate tuple
        /// </summary>
        Coordinates<short, short, short, short> Coordinates { get; }
    }

    /// <summary>
    ///     General interface for all 64 bit encoded linear 4D information that support copy factories
    /// </summary>
    /// <typeparam name="TVector"></typeparam>
    public interface ILinearVector4S<out TVector> : ILinearVector4S where TVector : struct, ILinearVector4S<TVector>
    {
        /// <summary>
        ///     Factory function to create new 4D vectors of this type
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        TVector CreateNew(short a, short b, short c, short p);
    }
}