using System;

namespace Mocassin.Mathematics.ValueTypes
{
    /// <summary>
    /// General interface for all 128 bit encoded linear 4D informations
    /// </summary>
    public interface ILinearVector4D
    {
        /// <summary>
        /// The 32 bit integer coordinate tuple
        /// </summary>
        Coordinates<Int32, Int32, Int32, Int32> Coordinates { get; }
    }

    /// <summary>
    /// General interface for all 128 bit encoded linear 4D informations that support copy factories
    /// </summary>
    /// <typeparam name="TVector"></typeparam>
    public interface ILinearVector4D<TVector> : ILinearVector4D where TVector : struct, ILinearVector4D<TVector>
    {
        /// <summary>
        /// Factory function to create new 4D vectors of this type
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        TVector CreateNew(Int32 a, Int32 b, Int32 c, Int32 p);
    }
}
