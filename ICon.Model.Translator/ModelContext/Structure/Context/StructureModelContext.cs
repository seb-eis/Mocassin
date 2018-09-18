using ICon.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Implementation of a structure model context that contains the full structure data context for simulation generation/evaluation
    /// </summary>
    public class StructureModelContext : IStructureModelContext
    {
        /// <summary>
        /// Describes the position models for a single unit cell with extended wyckoff positions
        /// </summary>
        public IUnitCellProvider<IPositionModel> PositionModels { get; set; }

        /// <summary>
        /// The list of existing environment models. Each for one unit cell position
        /// </summary>
        public IList<IEnvironmentModel> EnvironmentModels { get; set; }
    }
}
