using System;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.Coordinates
{
    /// <summary>
    ///     Abstract class that defines a coordinate system in N dimensional space
    /// </summary>
    public abstract class CoordinateSystem
    {
        /// <summary>
        ///     Dimensions value of the coordinate system
        /// </summary>
        public abstract int Dimension { get; }

        /// <summary>
        ///     Type of the coordinate structure
        /// </summary>
        public abstract Type CoordinateType { get; }
    }

    /// <summary>
    ///     Abstract class that defines a generic coordinate system in N dimensional space with a specific <see cref="ICoordinates"/> type
    /// </summary>
    public abstract class CoordinateSystem<T1> : CoordinateSystem where T1 : struct, ICoordinates
    {
        /// <summary>
        ///     Stores the dimension value that is defined in the coordinate tuple type
        /// </summary>
        private static readonly int DimensionValue = default(T1).Dimension;

        /// <inheritdoc />
        public override int Dimension => DimensionValue;

        /// <inheritdoc />
        public override Type CoordinateType => typeof(T1);

        /// <summary>
        ///     Get the type of the reference affine coordinate system this system is based upon
        /// </summary>
        public abstract Type ReferenceSystemType { get; }

        /// <summary>
        ///     Threats a coordinate tuple as being of the system type and transforms it to the specified reference type
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public abstract T1 ToReferenceSystem(in T1 original);

        /// <summary>
        ///     Threats a coordinate tuple as being of the reference type and transforms it to the specified system type
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public abstract T1 ToSystem(in T1 original);
    }
}