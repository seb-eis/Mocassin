using System;
using ICon.Framework.Exceptions;
using ACoordinates = ICon.Mathematics.ValueTypes.Coordinates<double, double, double>;

namespace ICon.Mathematics.Coordinates
{
    /// <summary>
    ///     Double precision cartesian coordinate system
    /// </summary>
    public class CartesianCoordinateSystem : AffineCoordinateSystem3D<ACoordinates>
    {
        /// <summary>
        ///     The system base coordinate vectors as coordinate tuple information
        /// </summary>
        public static readonly (ACoordinates A, ACoordinates B, ACoordinates C) BaseCoordinates;

        /// <inheritdoc />
        public override (ACoordinates A, ACoordinates B, ACoordinates C) BaseVectors
        {
            get => BaseCoordinates;
            protected set => 
                throw new InvalidStateChangeException("Cartesian base or reference vectors are constant and cannot be set");
        }

        /// <inheritdoc />
        public override (ACoordinates A, ACoordinates B, ACoordinates C) ReferenceBaseVectors
        {
            get => BaseCoordinates;
            protected set => 
                throw new InvalidStateChangeException("Cartesian base or reference vectors are constant and cannot be set");
        }

        /// <summary>
        ///     Static constructor, initializes the static cartesian base vectors
        /// </summary>
        static CartesianCoordinateSystem()
        {
            BaseCoordinates = (new ACoordinates(1.0, 0.0, 0.0), new ACoordinates(0.0, 1.0, 0.0), new ACoordinates(0.0, 0.0, 1.0));
        }


        /// <inheritdoc />
        public override Type ReferenceSystemType => typeof(CartesianCoordinateSystem);

        /// <inheritdoc />
        public override ACoordinates ToReferenceSystem(in ACoordinates original)
        {
            return original;
        }

        /// <inheritdoc />
        public override ACoordinates ToSystem(in ACoordinates original)
        {
            return original;
        }
    }
}