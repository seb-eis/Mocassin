using ICon.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class StructureModelContext : IStructureModelContext
    {
        /// <inheritdoc />
        public IUnitCellProvider<IPositionModel> PositionModels { get; set; }

        /// <inheritdoc />
        public IList<IEnvironmentModel> EnvironmentModels { get; set; }
    }
}
