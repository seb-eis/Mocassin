using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Simulations
=======
﻿namespace Mocassin.Model.Simulations
>>>>>>> origin/s.eisele@dev
{
    /// <summary>
    /// A lattice blueprint that describes a building instruction to create a custom lattice
    /// </summary>
    public class LatticeBlueprint : ILatticeBlueprint
    {
        public Random CustomRng { get; set; }

        public bool UseCustomBase { get; set; }

        public VectorInt3D SizeVector { get; set; }

        public IDictionary<IDoping, double> DopingConcentrations { get; set; }

    }
}