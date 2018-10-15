using System;
using ICon.Framework.Exceptions;

using ACoordinates = ICon.Mathematics.ValueTypes.Coordinates<double, double, double>;

namespace ICon.Mathematics.Coordinates
{
    /// <summary>
    /// Double precision cartesian coordinate system
    /// </summary>
    public class CartesianCoordinateSystem : AffineCoordinateSystem3D<ACoordinates>
    {
        /// <summary>
        /// The system base coordinate vectors as coordinate tuple information
        /// </summary>
        public static readonly (ACoordinates A, ACoordinates B, ACoordinates C) BaseCoordinates;

        /// <summary>
        /// Gets the trivial basis vectors of the cartesian system (Setter not implemented due to trivial case)
        /// </summary>
        public override (ACoordinates A, ACoordinates B, ACoordinates C) BaseVectors
        {
            get { return BaseCoordinates; }
            protected set { throw new InvalidStateChangeException("Cartesian base or refernce vectors are constant and cannot be set"); }
        }

        /// <summary>
        /// Get the trivial basis vectors of the reference system (Setter not implemented due to trivial case)
        /// </summary>
        public override (ACoordinates A, ACoordinates B, ACoordinates C) ReferenceBaseVectors
        {
            get { return BaseCoordinates; }
            protected set { throw new InvalidStateChangeException("Cartesian base or refernce vectors are constant and cannot be set"); }
        }

        /// <summary>
        /// Static constructor, initializes the static cartesian base vectors
        /// </summary>
        static CartesianCoordinateSystem()
        {
            BaseCoordinates = (new ACoordinates(1.0, 0.0, 0.0), new ACoordinates(0.0, 1.0, 0.0), new ACoordinates(0.0, 0.0, 1.0));
        }


        /// <summary>
        /// The refence system (For all 3D affine systems this is equal to the cartesian coordinate system)
        /// </summary>
        public override Type ReferenceCoorSystemType => typeof(CartesianCoordinateSystem);

        /// <summary>
        /// Trivial double transform of cartesian vector into cartesian vector
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public override ACoordinates TransformToReference(ACoordinates original)
        {
            return original;
        }

        /// <summary>
        /// Trivial double transform of cartesian vector into cartesian vector
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public override ACoordinates TransformToSystem(ACoordinates original)
        {
            return original;
        }
    }
}
