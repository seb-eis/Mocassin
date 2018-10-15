using System;

using ICon.Framework.Extensions;
using ICon.Mathematics.Extensions;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Class for a soft cubic crystal system that supports the crytsal system hierachy (i.e. the hard cubic condition is not enforced)
    /// </summary>
    public class SoftCubicSystem : CrystalSystem
    {
        /// <summary>
        /// Corrects the cubic angle and parameter dependencies in a parameter set
        /// </summary>
        /// <param name="paramSet"></param>
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
            paramSet.ParamB = paramSet.ParamA;
            paramSet.ParamC = paramSet.ParamA;
            paramSet.Alpha = MocassinMath.Radian90;
            paramSet.Beta = MocassinMath.Radian90;
            paramSet.Gamma = MocassinMath.Radian90;
        }

        /// <summary>
        /// Validates that the passed parameters fulfill the sufficient cubic condition (All 0.5*PI)
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public override Boolean ValidateSoftAngleCondition(Double alpha, Double beta, Double gamma)
        {
            return MocassinMath.Radian90.CountMatches(BasicConstraint.Comparer, alpha, beta, gamma) == 3;
        }

        /// <summary>
        /// Validates that the passed angles fulfill the sufficient cubic condition (All the same)
        /// </summary>
        /// <param name="paramA"></param>
        /// <param name="paramB"></param>
        /// <param name="paramC"></param>
        /// <returns></returns>
        public override Boolean ValidateSoftParameterCondition(Double paramA, Double paramB, Double paramC)
        {
            return paramA.CountMatches(BasicConstraint.Comparer, paramB, paramC) == 3;
        }

        /// <summary>
        /// Get a default valid cubic parameter set
        /// </summary>
        /// <returns></returns>
        public override CrystalParameterSet GetDefaultParameterSet()
        {
            return new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian90);
        }
    }
}
