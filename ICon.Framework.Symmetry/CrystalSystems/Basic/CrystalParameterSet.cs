using System.Runtime.Serialization;
using System.Xml.Serialization;
using Mocassin.Mathematics.Extensions;
using Newtonsoft.Json;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <summary>
    ///     Defines a set of lattice parameters values a, b, c with angles alpha, beta, gamma without the crystal context or fixing information
    /// </summary>
    [XmlRoot]
    public class CrystalParameterSet
    {
        /// <summary>
        ///     The parameter A
        /// </summary>
        public double ParamA { get; set; }

        /// <summary>
        ///     The parameter B
        /// </summary>
        public double ParamB { get; set; }

        /// <summary>
        ///     The parameter C
        /// </summary>
        public double ParamC { get; set; }

        /// <summary>
        ///     Angle alpha in radian
        /// </summary>
        public double Alpha { get; set; }

        /// <summary>
        ///     Angle beta in radian
        /// </summary>
        public double Beta { get; set; }

        /// <summary>
        ///     Angle gamma in radian
        /// </summary>
        public double Gamma { get; set; }

        /// <summary>
        ///     Create new crystal parameter set with default values
        /// </summary>
        public CrystalParameterSet()
        {
        }

        /// <summary>
        ///     Creates a new crystal parameter set
        /// </summary>
        /// <param name="paramA"></param>
        /// <param name="paramB"></param>
        /// <param name="paramC"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="gamma"></param>
        public CrystalParameterSet(double paramA, double paramB, double paramC, double alpha, double beta, double gamma)
        {
            ParamA = paramA;
            ParamB = paramB;
            ParamC = paramC;
            Alpha = alpha;
            Beta = beta;
            Gamma = gamma;
        }

        /// <summary>
        ///     Returns a json representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        ///     Creates default crystal parameter set (All params 1.0, all angles 90° in radian)
        /// </summary>
        /// <returns></returns>
        public static CrystalParameterSet CreateDefault()
        {
            return new CrystalParameterSet(1.0, 1.0, 1.0, MocassinMath.Radian90, MocassinMath.Radian90, MocassinMath.Radian90);
        }
    }
}