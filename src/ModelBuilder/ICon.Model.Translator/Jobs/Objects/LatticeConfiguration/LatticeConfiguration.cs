using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Mocassin.Model.Lattices;
using Moccasin.Mathematics.ValueTypes;

namespace Mocassin.Model.Translator.Jobs
{
    /// <summary>
    ///     A lattice configuration for a simulation job that carries lattice build instructions
    /// </summary>
    public class LatticeConfiguration
    {
        /// <summary>
        ///     Get or set the size in cell direction A
        /// </summary>
        public int SizeA { get; set; }

        /// <summary>
        ///     Get or set the size in cell direction B
        /// </summary>
        public int SizeB { get; set; }

        /// <summary>
        ///     Get or set the size in cell direction C
        /// </summary>
        public int SizeC { get; set; }

        /// <summary>
        ///     Get or set the doping concentrations
        /// </summary>
        public IDictionary<IDoping, double> DopingConcentrations { get; set; }

        /// <summary>
        ///     Copies the information to another lattice configuration
        /// </summary>
        /// <param name="latticeConfiguration"></param>
        public void CopyTo(LatticeConfiguration latticeConfiguration)
        {
            latticeConfiguration.SizeA = SizeA;
            latticeConfiguration.SizeB = SizeB;
            latticeConfiguration.SizeC = SizeC;
            latticeConfiguration.DopingConcentrations = new Dictionary<IDoping, double>(DopingConcentrations);
        }

        /// <summary>
        ///     Get the size as an <see cref="VectorI3" />
        /// </summary>
        /// <returns></returns>
        public VectorI3 GetSizeVector() => new VectorI3(SizeA, SizeB, SizeC);

        /// <summary>
        ///     Get set size as a default formatted <see cref="string" />
        /// </summary>
        /// <returns></returns>
        public string GetSizeString() => $"{SizeA},{SizeB},{SizeC}";

        /// <summary>
        ///     Get set size as a default formatted <see cref="string" /> including the position count
        /// </summary>
        /// <param name="sizeP"></param>
        /// <returns></returns>
        public string GetSizeString(int sizeP) => $"{SizeA},{SizeB},{SizeC},{sizeP}";

        /// <summary>
        ///     Get the set dopings as a default formatted <see cref="string" />
        /// </summary>
        /// <returns></returns>
        public string GetDopingString()
        {
            return DopingConcentrations
                .Aggregate("", (current, item) => current + $"[{item.Key.Index}@{item.Value.ToString(CultureInfo.InvariantCulture)}]");
        }
    }
}