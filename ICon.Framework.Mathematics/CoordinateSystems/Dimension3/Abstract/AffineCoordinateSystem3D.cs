﻿using System;

using ICon.Mathematics.ValueTypes;

namespace ICon.Mathematics.Coordinates
{
    /// <summary>
    /// Defines an affine coordinate system of the specified coordinate tuple type
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public abstract class AffineCoordinateSystem3D<T1> : CoordinateSystem<T1> where T1 : struct, ICoordinates
    {
        /// <summary>
        /// The coordinate tuples that represent the basis vector coordinate information of the affine system
        /// </summary>
        public abstract (T1 A, T1 B, T1 C) BaseVectors { get; protected set; }

        /// <summary>
        /// The coordinate tuples that represent the basis vector coordinate information of the affine refernce system
        /// </summary>
        public abstract (T1 A, T1 B, T1 C) ReferenceBaseVectors { get; protected set; }
    }
}
