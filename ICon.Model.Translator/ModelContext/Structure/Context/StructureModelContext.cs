using Mocassin.Symmetry.Analysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class StructureModelContext : IStructureModelContext
    {
        /// <inheritdoc />
        public IList<IPositionModel> PositionModels { get; set; }

        /// <inheritdoc />
        public IList<IEnvironmentModel> EnvironmentModels { get; set; }
    }
}
