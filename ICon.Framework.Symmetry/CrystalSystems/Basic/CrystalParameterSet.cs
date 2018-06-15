using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using ICon.Mathematics.Extensions;

namespace ICon.Symmetry.CrystalSystems
{
    /// <summary>
    /// Full set of lattice parameters a, b, c and angles alph, beta, gamma that is serializable
    /// </summary>
    [DataContract(Name ="CrystalParamSet")]
    public class CrystalParameterSet
    {
        /// <summary>
        /// The parameter A
        /// </summary>
        [DataMember(Name ="A")]
        public double ParamA { get; set; }

        /// <summary>
        /// The parameter B
        /// </summary>
        [DataMember(Name ="B")]
        public double ParamB { get; set; }

        /// <summary>
        /// The parameter C
        /// </summary>
        [DataMember(Name ="C")]
        public double ParamC { get; set; }

        /// <summary>
        /// Angle alpha in radian
        /// </summary>
        [DataMember(Name ="Alpha")]
        public double Alpha { get; set; }

        /// <summary>
        /// Angle beta in radian
        /// </summary>
        [DataMember(Name ="Beta")]
        public double Beta { get; set; }

        /// <summary>
        /// Angle gamma in radian
        /// </summary>
        [DataMember(Name ="Gamma")]
        public double Gamma { get; set; }

        /// <summary>
        /// Create new crystal parameter set with default values
        /// </summary>
        public CrystalParameterSet()
        {
        }

        /// <summary>
        /// Creates a new crystal parameter set
        /// </summary>
        /// <param name="paramA"></param>
        /// <param name="paramB"></param>
        /// <param name="paramC"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        public CrystalParameterSet(Double paramA, Double paramB, Double paramC, Double alpha, Double beta, Double gamma)
        {
            ParamA = paramA;
            ParamB = paramB;
            ParamC = paramC;
            Alpha = alpha;
            Beta = beta;
            Gamma = gamma;
        }

        /// <summary>
        /// Retruns a json representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Creates default crystal parameter set (All params 1.0, all angles 90° in radian)
        /// </summary>
        /// <returns></returns>
        public static CrystalParameterSet CreateDefault()
        {
            return new CrystalParameterSet(1.0, 1.0, 1.0, ExtMath.Radian90, ExtMath.Radian90, ExtMath.Radian90);
        }
    }
}
