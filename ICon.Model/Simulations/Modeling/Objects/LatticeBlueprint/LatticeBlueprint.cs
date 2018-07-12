using System;
using System.Collections.Generic;
using System.Text;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Lattices;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// A lattice blueprint that describes a building instruction to create a custom lattice
    /// </summary>
    public class LatticeBlueprint : ILatticeBlueprint
    {
        public Random CustomRng { get; set; }

        public bool UseCustomBase { get; set; }

        public CartesianInt3D SizeVector { get; set; }

        public IDictionary<IDoping, double> DopingConcentrations { get; set; }

    }
}
