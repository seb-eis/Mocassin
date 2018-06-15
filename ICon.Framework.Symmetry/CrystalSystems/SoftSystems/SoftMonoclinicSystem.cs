using System;
using System.Linq;

using ICon.Mathematics.Extensions;
using ICon.Framework.Extensions;
using ICon.Framework.Exceptions;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Class for a soft monoclinic crystal system that supports the crytsal system hierachy (i.e. the hard monoclinic condition is not enforced)
    /// </summary>
    public class SoftMonoclinicSystem : CrystalSystem
    {
        /// <summary>
        /// Corrects parameter and angle dependencies of the monoclinic system (Unqiue axis angle corrections)
        /// </summary>
        /// <param name="paramSet"></param>
        public override void ApplyParameterDependencies(CrystalParameterSet paramSet)
        {
            if (Variation == CrystalVariation.UniqueAxisA)
            {
                paramSet.Beta = paramSet.Alpha;
                paramSet.Gamma = paramSet.Alpha;
                return;
            }
            if (Variation == CrystalVariation.UniqueAxisB)
            {
                paramSet.Alpha = paramSet.Beta;
                paramSet.Gamma = paramSet.Beta;
                return;
            }
            if (Variation == CrystalVariation.UniqueAxisC)
            {
                paramSet.Alpha = paramSet.Gamma;
                paramSet.Beta = paramSet.Gamma;
                return;
            }
            throw new InvalidObjectStateException("The monoclinic crystal system does not have a valid unique axis flag");
        }

        /// <summary>
        /// Validates the soft monoclinic angle condition (two specific angles equal to PI * 0.5, depends on unique axis variation)
        /// </summary>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        /// <returns></returns>
        public override Boolean ValidateSoftAngleCondition(Double alpha, Double beta, Double gamma)
        {
            if (Variation == CrystalVariation.UniqueAxisA)
            {
                return ExtMath.Radian90.CountMatches(BasicConstraint.Comparer, beta, gamma) == 2;
            }
            if (Variation == CrystalVariation.UniqueAxisB)
            {
                return ExtMath.Radian90.CountMatches(BasicConstraint.Comparer, beta, gamma) == 2;
            }
            if (Variation == CrystalVariation.UniqueAxisC)
            {
                return ExtMath.Radian90.CountMatches(BasicConstraint.Comparer, beta, gamma) == 2;
            }
            throw new InvalidObjectStateException("The monoclinic crystal system has an invalid crystal system variation flag");
        }

        /// <summary>
        /// Validates that the passed parameters fulfill the monoclinic parameter condition (always true)
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
        /// Get a default valid monoclinic parameter set
        /// </summary>
        /// <returns></returns>
        public override CrystalParameterSet GetDefaultParameterSet()
        {
            return new CrystalParameterSet(1.0, 1.0, 1.0, ExtMath.Radian90, ExtMath.Radian90, ExtMath.Radian90);
        }
    }
}
