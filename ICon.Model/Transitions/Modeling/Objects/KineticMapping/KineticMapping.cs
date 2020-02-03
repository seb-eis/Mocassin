using System;
using System.Linq;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Transitions
{
    /// <summary>
    ///     Mapping for a kinetic transition that describes a 4D encoded transition path belonging to a specific kinetic
    ///     reference transition
    /// </summary>
    public readonly struct KineticMapping
    {
        /// <summary>
        ///     The unit cell position that describes the start point
        /// </summary>
        public ICellReferencePosition StartCellReferencePosition { get; }

        /// <summary>
        ///     The unit cell position that describes the end point
        /// </summary>
        public ICellReferencePosition EndCellReferencePosition { get; }

        /// <summary>
        ///     The interface of the transition the mapping is valid for
        /// </summary>
        public IKineticTransition Transition { get; }

        /// <summary>
        ///     The transition path encoded as a set of 4D vectors (Cannot be null)
        /// </summary>
        public CrystalVector4D[] EncodedPath { get; }

        /// <summary>
        ///     The transition path in fractional coordinates (Can be null)
        /// </summary>
        public Fractional3D[] FractionalPath { get; }

        /// <summary>
        ///     The path length of the mapping
        /// </summary>
        public int PathLength => Transition.GeometryStepCount;

        /// <summary>
        ///     The 4D encoded start position vector of the mapping
        /// </summary>
        public CrystalVector4D StartVector4D => EncodedPath[0];

        /// <summary>
        ///     The 4D encoded end position vector of the mapping
        /// </summary>
        public CrystalVector4D EndVector4D => EncodedPath[PathLength - 1];

        /// <summary>
        ///     The 3D fractional start position vector of the mapping
        /// </summary>
        public Fractional3D StartVector3D => FractionalPath[0];

        /// <summary>
        ///     The 3D fractional end position vector of the mapping
        /// </summary>
        public Fractional3D EndVector3D => FractionalPath[PathLength - 1];

        /// <summary>
        ///     Create a new kinetic mapping from transition interface, cell positions, 4D encoded transition path and fractional
        ///     path
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="startCellReferencePosition"></param>
        /// <param name="endCellReferencePosition"></param>
        /// <param name="encodedPath"></param>
        /// <param name="fractionalPath"></param>
        public KineticMapping(IKineticTransition transition, ICellReferencePosition startCellReferencePosition, ICellReferencePosition endCellReferencePosition,
            CrystalVector4D[] encodedPath, Fractional3D[] fractionalPath)
            : this()
        {
            Transition = transition;
            StartCellReferencePosition = startCellReferencePosition;
            EndCellReferencePosition = endCellReferencePosition;
            EncodedPath = encodedPath ?? throw new ArgumentNullException(nameof(encodedPath));
            FractionalPath = fractionalPath;
        }

        /// <inheritdoc />
        public KineticMapping(IKineticTransition transition, CrystalVector4D[] encodedPath)
            : this(transition, null, null, encodedPath, null)
        {
        }

        /// <summary>
        ///     Creates an inverted kinetic mapping
        /// </summary>
        /// <returns></returns>
        public KineticMapping CreateGeometricInversion()
        {
            var inverseEncodedPath = EncodedPath.Reverse().ToArray();
            var inverseFractionalPath = FractionalPath.Reverse().ToArray();
            return new KineticMapping(
                Transition, EndCellReferencePosition, StartCellReferencePosition, inverseEncodedPath, inverseFractionalPath
            );
        }
    }
}