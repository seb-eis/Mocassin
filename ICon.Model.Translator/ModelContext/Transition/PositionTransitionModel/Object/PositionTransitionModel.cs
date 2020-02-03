using System.Collections.Generic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IPositionTransitionModel" />
    public class PositionTransitionModel : ModelComponentBase, IPositionTransitionModel
    {
        /// <inheritdoc />
        public ICellReferencePosition CellReferencePosition { get; set; }

        /// <inheritdoc />
        public IList<IKineticTransitionModel> KineticTransitionModels { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisTransitionModel> MetropolisTransitionModels { get; set; }
    }
}