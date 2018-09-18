using System;
using System.Collections.Generic;
using System.Text;
using ICon.Model.Structures;
using ICon.Symmetry.SpaceGroups;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Environment model implementation that carries all extended structure and interaction information for a single unit cell position
    /// </summary>
    public class EnvironmentModel : ModelComponentBase, IEnvironmentModel
    {
        /// <summary>
        /// The unit cell position that the environment is based upon
        /// </summary>
        public IUnitCellPosition UnitCellPosition { get; set; }

        /// <summary>
        /// The list of pair interaction models that belong to the environment
        /// </summary>
        public IList<IPairInteractionModel> PairInteractionModels { get; set; }

        /// <summary>
        /// The list of group interaction models that belong to the environment
        /// </summary>
        public IList<IGroupInteractionModel> GroupInteractionModels { get; set; }

        /// <summary>
        /// Wyckoff operation dictionary that contains all transform operations that reach equivalent center positions
        /// </summary>
        public IWyckoffOperationDictionary TransformOperations { get; set; }
    }
}
