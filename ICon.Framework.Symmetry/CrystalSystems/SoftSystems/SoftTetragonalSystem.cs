using System;

using ICon.Framework.Extensions;
using ICon.Mathematics.Extensions;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Class for a soft tetragonal crystal system that supports the crytsal system hierachy (i.e. the hard tetragonal condition is not enforced)
    /// </summary>
    public class SoftTetragonalSystem : CrystalSystem
    {
        /// <summary>
        /// Corrects the tetragonal angle and parameter dependencies in a parameter set
        /// </summary>
        /// <param name="paramSet"></param>
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
            paramSet.Alpha = MocassinMath.Radian90;
            paramSet.Beta = MocassinMath.Radian90;
            paramSet.Gamma = MocassinMath.Radian90;
            paramSet.ParamB = paramSet.ParamB;
        }

        /// <summary>
        /// Validates that the passed parameters fulfill the sufficient tetragonal condition (All angles 0.5*PI)
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
        /// Validates that the passed angles fulfill the sufficient tetragonal condition (b == c)
        /// </summary>
        /// <param name="paramA"></param>
        /// <param name="paramB"></param>
        /// <param name="paramC"></param>
        /// <returns></returns>
        public override Boolean ValidateSoftParameterCondition(Double paramA, Double paramB, Double paramC)
        {
            return BasicConstraint.Comparer.Equals(paramA, paramB);
        }

        /// <summary>
        /// Get a default valid tetragonal parameter set
        /// </summary>
        /// <returns></returns>
        public override CrystalParameterSet GetDefaultParameterSet()
        {
            return new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian90);
        }
    }
}
