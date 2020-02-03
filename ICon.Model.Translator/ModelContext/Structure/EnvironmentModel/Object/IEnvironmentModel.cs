using System.Collections.Generic;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Represents an environment model that fully describes the surroundings and dependencies of a unit cell position
    /// </summary>
    public interface IEnvironmentModel : IModelComponent
    {
        /// <summary>
        ///     The unit cell position that the environment is based upon
        /// </summary>
        ICellReferencePosition CellReferencePosition { get; set; }

        /// <summary>
        ///     The list of pair interaction models that belong to the environment
        /// </summary>
        IList<IPairInteractionModel> PairInteractionModels { get; set; }

        /// <summary>
        ///     The list of group interaction models that belong to the environment
        /// </summary>
        IList<IGroupInteractionModel> GroupInteractionModels { get; set; }

        /// <summary>
        ///     Wyckoff operation dictionary that contains all transform operations that reach equivalent center positions
        /// </summary>
        IPositionOperationDictionary TransformOperations { get; set; }
    }
}