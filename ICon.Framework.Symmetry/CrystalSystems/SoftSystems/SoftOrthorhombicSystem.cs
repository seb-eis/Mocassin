using System;

using ICon.Framework.Extensions;
using ICon.Mathematics.Extensions;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Class for a soft orthorhombic crystal system that supports the crytsal system hierachy (i.e. the hard orthorhombic condition is not enforced)
    /// </summary>
    public class SoftOrthorhombicSystem : CrystalSystem
    {
        /// <summary>
        /// Corrects the orthorhombic parameter and angle dependencies in a parameter set
        /// </summary>
        /// <param name="paramSet"></param>
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
            paramSet.Alpha = ExtMath.Radian90;
            paramSet.Beta = ExtMath.Radian90;
            paramSet.Gamma = ExtMath.Radian90;
        }

        /// <summary>
        /// Validates that the passed parameters fulfill the sufficient orthorhombic condition (all angles are equal to 0.5*PI)
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public override Boolean ValidateSoftAngleCondition(Double alpha, Double beta, Double gamma)
        {
            return ExtMath.Radian90.CountMatches(BasicConstraint.Comparer, alpha, beta, gamma) == 3;
        }

        /// <summary>
        /// Validates that the passed angles fulfill the sufficient orthorhombic condition (Always true)
        /// </summary>
        /// <param name="paramA"></param>
        /// <param name="paramB"></param>
        /// <param name="paramC"></param>
        /// <returns></returns>
        public override Boolean ValidateSoftParameterCondition(Double paramA, Double paramB, Double paramC)
        {
            return true;
        }

        /// <summary>
        /// Get a default valid orthorhombic parameter set
        /// </summary>
        /// <returns></returns>
        public override CrystalParameterSet GetDefaultParameterSet()
        {
            return new CrystalParameterSet(1.0, 1.0, 1.0, ExtMath.Radian90, ExtMath.Radian90, ExtMath.Radian90);
        }
    }
}
