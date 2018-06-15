using System;

using ICon.Mathematics.Extensions;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Class for a soft hexagonal crystal system that supports the crytsal system hierachy (i.e. the hard hexagonal condition is not enforced)
    /// </summary>
    public class SoftHexagonalSystem : CrystalSystem
    {
        /// <summary>
        /// Corrects the hexagonal parameter and angle dependencies for a parameter set
        /// </summary>
        /// <param name="paramSet"></param>
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
            paramSet.ParamB = paramSet.ParamA;
            paramSet.Alpha = ExtMath.Radian90;
            paramSet.Beta = ExtMath.Radian90;
            paramSet.Gamma = ExtMath.Radian120;
        }

        /// <summary>
        /// Validates that the passed parameters fulfill the sufficient hexagonal condition (0.5*PI, 0.5*PI, 2/3*PI)
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public override Boolean ValidateSoftAngleCondition(Double alpha, Double beta, Double gamma)
        {
            Boolean isValid = true;
            isValid &= BasicConstraint.Comparer.Equals(alpha, ExtMath.Radian90);
            isValid &= BasicConstraint.Comparer.Equals(beta, ExtMath.Radian90);
            isValid &= BasicConstraint.Comparer.Equals(gamma, ExtMath.Radian120);
            return isValid;
        }

        /// <summary>
        /// Validates that the passed angles fulfill the sufficient hexagonal condition (a equals b)
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
        /// Get a default valid hexagonal parameter set
        /// </summary>
        /// <returns></returns>
        public override CrystalParameterSet GetDefaultParameterSet()
        {
            return new CrystalParameterSet(1.0, 1.0, 1.0, ExtMath.Radian90, ExtMath.Radian90, ExtMath.Radian120);
        }
    }
}
