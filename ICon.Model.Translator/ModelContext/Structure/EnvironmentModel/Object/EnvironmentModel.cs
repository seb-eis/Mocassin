using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <inheritdoc cref="IEnvironmentModel"/>
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
