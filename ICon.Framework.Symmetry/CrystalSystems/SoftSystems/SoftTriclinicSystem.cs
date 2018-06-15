using System;

using ICon.Mathematics.Extensions;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Class for a soft triclinic crystal system that supports the crytsal system hierachy (i.e. the hard triclinic condition is not enforced)
    /// </summary>
    public class SoftTriclinicSystem : CrystalSystem
    {
        /// <summary>
        /// Corrects parameter and angle dependencies of the triclinic system
        /// </summary>
        /// <param name="paramSet"></param>
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
            
        }

        /// <summary>
        /// Validates that the passed parameters fulfill the sufficient triclinc condition (Always true)
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public override Boolean ValidateSoftAngleCondition(Double alpha, Double beta, Double gamma)
        {
            return true;
        }

        /// <summary>
        /// Validates that the passed angles fulfill the sufficient triclinc condition (Always true)
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
        /// Get a default valid triclinic parameter set
        /// </summary>
        /// <returns></returns>
        public override CrystalParameterSet GetDefaultParameterSet()
        {
            return new CrystalParameterSet(1.0, 1.0, 1.0, ExtMath.Radian90, ExtMath.Radian90, ExtMath.Radian90);
        }
    }
}
