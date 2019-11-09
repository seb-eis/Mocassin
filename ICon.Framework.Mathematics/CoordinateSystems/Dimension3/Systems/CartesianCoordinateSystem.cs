using System;
using Mocassin.Framework.Exceptions;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Mathematics.Coordinates
{
    /// <summary>
    ///     Double precision cartesian coordinate system
    /// </summary>
    public class CartesianCoordinateSystem : AffineCoordinateSystem3D<Coordinates3D>
    {
        /// <summary>
        ///     The system base coordinate vectors as coordinate tuple information
        /// </summary>
        public static readonly (Coordinates3D A, Coordinates3D B, Coordinates3D C) BaseCoordinates;

        /// <inheritdoc />
        public override (Coordinates3D A, Coordinates3D B, Coordinates3D C) BaseVectors => BaseCoordinates;

        /// <inheritdoc />
        public override (Coordinates3D A, Coordinates3D B, Coordinates3D C) ReferenceBaseVectors => BaseCoordinates;

        /// <summary>
        ///     Static constructor, initializes the static cartesian base vectors
        /// </summary>
        static CartesianCoordinateSystem()
        {
            BaseCoordinates = (new Coordinates3D(1.0, 0.0, 0.0), new Coordinates3D(0.0, 1.0, 0.0), new Coordinates3D(0.0, 0.0, 1.0));
        }


        /// <inheritdoc />
        public override Type ReferenceSystemType => typeof(CartesianCoordinateSystem);

        /// <inheritdoc />
        public override Coordinates3D ToReferenceSystem(in Coordinates3D original)
        {
            return original;
        }

        /// <inheritdoc />
        public override Coordinates3D ToSystem(in Coordinates3D original)
        {
            return original;
        }
    }
}