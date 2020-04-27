using System;
using System.Collections.Generic;
using System.Diagnostics;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.ValueTypes;

namespace Mocassin.Symmetry.Analysis
{
    /// <summary>
    ///     Describes the full geometric information of a target in a 4D position lattice
    /// </summary>
    [DebuggerDisplay("LatticeTarget: {DistanceInFm} fm, {CrystalVectorToTarget}")]
    public class LatticeTarget
    {
        /// <summary>
        ///     Get the exact distance in [Ang]
        /// </summary>
        public double Distance { get; private set; }

        /// <summary>
        ///     Get the distance rounded to [fm]
        /// </summary>
        public int DistanceInFm { get; private set; }

        /// <summary>
        ///     Get the absolute <see cref="Fractional3D" /> of the target
        /// </summary>
        public Fractional3D Fractional { get; private set; }

        /// <summary>
        ///     Get the relative <see cref="Fractional3D" /> to the target
        /// </summary>
        public Fractional3D FractionalToTarget { get; private set; }

        /// <summary>
        ///     Get the absolute <see cref="Cartesian3D" /> of the target
        /// </summary>
        public Cartesian3D Cartesian { get; private set; }

        /// <summary>
        ///     Get the relative <see cref="Cartesian3D" /> to the target
        /// </summary>
        public Cartesian3D CartesianToTarget { get; private set; }

        /// <summary>
        ///     Get the absolute <see cref="Vector4I" /> of the target
        /// </summary>

        public Vector4I CrystalVector { get; private set; }

        /// <summary>
        ///     Get the relative <see cref="Vector4I" /> to the target
        /// </summary>
        public Vector4I CrystalVectorToTarget { get; private set; }

        /// <summary>
        ///     Builds a new <see cref="LatticeTarget" /> from vector data
        /// </summary>
        /// <param name="sourceFractional"></param>
        /// <param name="sourceCartesian"></param>
        /// <param name="sourceCrystalVector"></param>
        /// <param name="targetFractional"></param>
        /// <param name="targetCartesian"></param>
        /// <param name="targetCrystalVector"></param>
        /// <returns></returns>
        public static LatticeTarget FromVectors(in Fractional3D sourceFractional, in Cartesian3D sourceCartesian, in Vector4I sourceCrystalVector,
            in Fractional3D targetFractional, in Cartesian3D targetCartesian, in Vector4I targetCrystalVector)
        {
            var result = new LatticeTarget
            {
                Fractional = targetFractional,
                Cartesian = targetCartesian,
                CrystalVector = targetCrystalVector,
                FractionalToTarget = targetFractional - sourceFractional,
                CartesianToTarget = targetCartesian - sourceCartesian,
                CrystalVectorToTarget = targetCrystalVector - sourceCrystalVector,
            };
            result.Distance = result.CartesianToTarget.GetLength();
            result.DistanceInFm = (int) Math.Round(result.Distance * 1e5, 0);
            return result;
        }

        /// <summary>
        ///     Builds a new <see cref="LatticeTarget" /> from source and target <see cref="LatticePoint{TContent}" /> and
        ///     <see cref="IUnitCellVectorEncoder" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static LatticeTarget FromLatticePoints<T>(in LatticePoint<T> source, in LatticePoint<T> target, IUnitCellVectorEncoder vectorEncoder)
        {
            var sourceCartesian = vectorEncoder.Transformer.ToCartesian(source.Fractional);
            var targetCartesian = vectorEncoder.Transformer.ToCartesian(target.Fractional);
            var sourceCrystalVector = vectorEncoder.TryEncode(source.Fractional, out var vector)
                ? vector
                : throw new InvalidOperationException("Could not encode source vector into 4D crystal vector");
            var targetCrystalVector = vectorEncoder.TryEncode(target.Fractional, out vector)
                ? vector
                : throw new InvalidOperationException("Could not encode source vector into 4D crystal vector");
            return FromVectors(source.Fractional, sourceCartesian, sourceCrystalVector, target.Fractional, targetCartesian, targetCrystalVector);
        }

        /// <summary>
        ///     Builds a sequence of <see cref="LatticeTarget" /> instances from a source and multiple target
        ///     <see cref="LatticePoint{TContent}" /> and a <see cref="IUnitCellVectorEncoder" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="targets"></param>
        /// <param name="vectorEncoder"></param>
        /// <returns></returns>
        public static IEnumerable<LatticeTarget> FromLatticePoints<T>(LatticePoint<T> source, IEnumerable<LatticePoint<T>> targets,
            IUnitCellVectorEncoder vectorEncoder)
        {
            var sourceCartesian = vectorEncoder.Transformer.ToCartesian(source.Fractional);
            var sourceCrystalVector = vectorEncoder.TryEncode(source.Fractional, out var vector)
                ? vector
                : throw new InvalidOperationException("Could not encode source vector into 4D crystal vector");
            foreach (var target in targets)
            {
                var targetCartesian = vectorEncoder.Transformer.ToCartesian(target.Fractional);
                var targetCrystalVector = vectorEncoder.TryEncode(target.Fractional, out vector)
                    ? vector
                    : throw new InvalidOperationException("Could not encode source vector into 4D crystal vector");
                yield return FromVectors(source.Fractional, sourceCartesian, sourceCrystalVector, target.Fractional, targetCartesian, targetCrystalVector);
            }
        }
    }
}