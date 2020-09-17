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
    public class KineticMapping
    {
        /// <summary>
        ///     The unit cell position that describes the start point
        /// </summary>
        public ICellSite StartCellSite { get; }

        /// <summary>
        ///     The unit cell position that describes the end point
        /// </summary>
        public ICellSite EndCellSite { get; }

        /// <summary>
        ///     The interface of the transition the mapping is valid for
        /// </summary>
        public IKineticTransition Transition { get; }

        /// <summary>
        ///     The transition path encoded as a set of 4D vectors (Cannot be null)
        /// </summary>
        public Vector4I[] EncodedPath { get; }

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
        public Vector4I StartVector4D => EncodedPath[0];

        /// <summary>
        ///     The 4D encoded end position vector of the mapping
        /// </summary>
        public Vector4I EndVector4D => EncodedPath[PathLength - 1];

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
        /// <param name="startCellSite"></param>
        /// <param name="endCellSite"></param>
        /// <param name="encodedPath"></param>
        /// <param name="fractionalPath"></param>
        public KineticMapping(IKineticTransition transition, ICellSite startCellSite, ICellSite endCellSite,
            Vector4I[] encodedPath, Fractional3D[] fractionalPath)
        {
            Transition = transition;
            StartCellSite = startCellSite;
            EndCellSite = endCellSite;
            EncodedPath = encodedPath ?? throw new ArgumentNullException(nameof(encodedPath));
            FractionalPath = fractionalPath;
        }

        /// <inheritdoc />
        public KineticMapping(IKineticTransition transition, Vector4I[] encodedPath)
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
                Transition, EndCellSite, StartCellSite, inverseEncodedPath, inverseFractionalPath
            );
        }
    }
}