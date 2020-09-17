using System.Collections.Generic;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc />
    public class StructureModelContext : IStructureModelContext
    {
        /// <inheritdoc />
        public IList<IPositionModel> PositionModels { get; set; }

        /// <inheritdoc />
        public IList<IEnvironmentModel> EnvironmentModels { get; set; }

        /// <inheritdoc />
        public IInteractionRangeModel InteractionRangeModel { get; set; }
    }
}