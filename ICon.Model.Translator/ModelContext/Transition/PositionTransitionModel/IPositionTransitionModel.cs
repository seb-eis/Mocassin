using ICon.Mathematics.ValueTypes;
using ICon.Model.Structures;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Represents a position transition model that describes which transitions are possible on a unit cell position
    /// </summary>
    public interface IPositionTransitionModel : IModelComponent
    {
        /// <summary>
        /// The unit cell position the position transition model belongs to
        /// </summary>
        IUnitCellPosition UnitCellPosition { get; set; }

        /// <summary>
        /// The list of possible kinetic transition models on this position
        /// </summary>
        IList<IKineticTransitionModel> KineticTransitionModels { get; set; }

        /// <summary>
        /// The list of possible metropolis transition models on this position
        /// </summary>
        IList<IMetropolisTransitionModel> MetropolisTransitionModels { get; set; }
    }
}
