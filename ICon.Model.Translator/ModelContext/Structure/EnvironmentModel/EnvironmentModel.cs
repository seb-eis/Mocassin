using System;
using System.Collections.Generic;
using System.Text;
using ICon.Model.Structures;
using ICon.Symmetry.SpaceGroups;

namespace ICon.Model.Translator.ModelContext
{
    /// <inheritdoc cref="ICon.Model.Translator.ModelContext.IEnvironmentModel"/>
    public class EnvironmentModel : ModelComponentBase, IEnvironmentModel
    {
        /// <inheritdoc />
        public IUnitCellPosition UnitCellPosition { get; set; }

        /// <inheritdoc />
        public IList<IPairInteractionModel> PairInteractionModels { get; set; }

        /// <inheritdoc />
        public IList<IGroupInteractionModel> GroupInteractionModels { get; set; }

        /// <inheritdoc />
        public IWyckoffOperationDictionary TransformOperations { get; set; }
    }
}
