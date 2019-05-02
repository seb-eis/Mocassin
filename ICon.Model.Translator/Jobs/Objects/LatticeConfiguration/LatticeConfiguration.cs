using System.Collections.Generic;
using System.Collections.Immutable;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Lattices;

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
		///		Get or set the doping concentrations
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

	    public DataIntVector3D GetIntVector3D()
	    {
			return new DataIntVector3D(SizeA, SizeB, SizeC);
	    }
    }
}