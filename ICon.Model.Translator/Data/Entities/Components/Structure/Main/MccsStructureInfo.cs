using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    /// <summary>
    /// The mccs structure component that carries all encoded simulation data affiliated with structure modelling
    /// </summary>
    public class MccsStructureInfo : MccsComponent
    {
        /// <summary>
        /// The list of cell positions and their surroundings for the simulation
        /// </summary>
        public List<Environment> CellPositionEntities { get; set; }
    }
}
