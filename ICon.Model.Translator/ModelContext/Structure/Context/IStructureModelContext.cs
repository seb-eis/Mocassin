using System;
using System.Collections.Generic;
using System.Text;
using ICon.Symmetry.Analysis;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents the model data context for structural information
    /// </summary>
    public interface IStructureModelContext
    {
        /// <summary>
        /// The position model set as a unit cell provider system
        /// </summary>
        IUnitCellProvider<IPositionModel> PositionModelUnitCellProvider { get; set; }

        /// <summary>
        /// Describes the position models for a single unit cell with extended wyckoff positions
        /// </summary>
        IList<IPositionModel> PositionModels { get; set; }

        /// <summary>
        /// The list of existing environment models. Each for one unit cell position
        /// </summary>
        IList<IEnvironmentModel> EnvironmentModels { get; set; }
    }
}
