using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Lattices;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    /// A lattice blueprint that describes a building instruction to create a custom lattice
    /// </summary>
    public class LatticeBlueprint : ILatticeBlueprint
    {
        public Random CustomRng { get; set; }

        public bool UseCustomBase { get; set; }

        public DataIntVector3D SizeVector { get; set; }

        public IDictionary<IDoping, double> DopingConcentrations { get; set; }

    }
}