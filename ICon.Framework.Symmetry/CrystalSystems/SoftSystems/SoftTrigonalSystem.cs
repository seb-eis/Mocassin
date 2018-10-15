using System;

using ICon.Framework.Extensions;
using ICon.Framework.Exceptions;
using ICon.Mathematics.Extensions;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Class for a soft trigonal crystal system that supports the crytsal system hierachy (i.e. the hard trigonal condition is not enforced)
    /// </summary>
    public class SoftTrigonalSystem : CrystalSystem
    {
        /// <summary>
        /// Corrects the trigonal parameter and angle dependencies for either hexagonal or rhobohedral axes type
        /// </summary>
        /// <param name="paramSet"></param>
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
            if (Variation == CrystalVariation.HexagonalAxes)
            {
                paramSet.ParamB = paramSet.ParamA;
                paramSet.Alpha = MocassinMath.Radian90;
                paramSet.Beta = MocassinMath.Radian90;
                paramSet.Gamma = MocassinMath.Radian120;
                return;
            }
            if (Variation == CrystalVariation.RhombohedralAxes)
            {
                paramSet.ParamB = paramSet.ParamA;
                paramSet.ParamC = paramSet.ParamA;
                paramSet.Beta = paramSet.Alpha;
                paramSet.Gamma = paramSet.Alpha;
                return;
            }
            throw new InvalidObjectStateException("The trigonal crystal system has an invalid crystal system variation flag");
        }

        /// <summary>
        /// Validates that the passed parameters fulfill the sufficient trigonal condition (hexagonal or rhombohedral)
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public override Boolean ValidateSoftAngleCondition(Double alpha, Double beta, Double gamma)
        {
            Boolean isValid = true;
            if (Variation == CrystalVariation.HexagonalAxes)
            {
                isValid &= BasicConstraint.Comparer.Equals(alpha, MocassinMath.Radian90);
                isValid &= BasicConstraint.Comparer.Equals(beta, MocassinMath.Radian90);
                isValid &= BasicConstraint.Comparer.Equals(gamma, MocassinMath.Radian120);
                return isValid;
            }
            if (Variation == CrystalVariation.RhombohedralAxes)
            {
                isValid = alpha.CountMatches(BasicConstraint.Comparer, beta, gamma) == 2;
                return isValid;
            }
            throw new InvalidObjectStateException("The trigonal crystal system has an invalid crystal system variation flag");
        }

        /// <summary>
        /// Validates that the passed angles fulfill the sufficient trigonal condition (hexagonal or rhombohedral)
        /// </summary>
        /// <param name="paramA"></param>
        /// <param name="paramB"></param>
        /// <param name="paramC"></param>
        /// <returns></returns>
        public override Boolean ValidateSoftParameterCondition(Double paramA, Double paramB, Double paramC)
        {
            if (Variation == CrystalVariation.HexagonalAxes)
            {
                return BasicConstraint.Comparer.Equals(paramA, paramB);
            }
            if (Variation == CrystalVariation.RhombohedralAxes)
            {
                return paramA.CountMatches(BasicConstraint.Comparer, paramB, paramC) == 2;
            }
            throw new InvalidObjectStateException("The trigonal crystal system has an invalid crystal system variation flag");
        }

        /// <summary>
        /// Get a default valid trigonal parameter set (Depends on axis choice)
        /// </summary>
        /// <returns></returns>
        public override CrystalParameterSet GetDefaultParameterSet()
        {
            if (Variation == CrystalVariation.HexagonalAxes)
            {
                return new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian120);
            }
            if (Variation == CrystalVariation.RhombohedralAxes)
            {
                return new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian90);
            }
            throw new InvalidObjectStateException("The trigonal crystal system has an invalid crystal system variation flag");
        }
    }
}
