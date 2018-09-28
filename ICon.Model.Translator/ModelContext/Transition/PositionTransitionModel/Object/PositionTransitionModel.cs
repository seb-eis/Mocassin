using System.Collections.Generic;
using ICon.Model.Structures;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IPositionTransitionModel"/>
    public class PositionTransitionModel : ModelComponentBase, IPositionTransitionModel
    {
        /// <inheritdoc />
        public IUnitCellPosition UnitCellPosition { get; set; }

        /// <inheritdoc />
        public IList<IKineticTransitionModel> KineticTransitionModels { get; set; }

        /// <inheritdoc />
        public IList<IMetropolisTransitionModel> MetropolisTransitionModels { get; set; }
    }
}