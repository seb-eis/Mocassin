using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.ConflictHandling
{
    /// <summary>
    ///     Conflict resolver for parameter induced conflicts within the structure manager
    /// </summary>
    public class StructureParameterConflictHandler : DataConflictHandler<StructureModelData, ModelParameter>
    {
        /// <inheritdoc />
        public StructureParameterConflictHandler(IModelProject modelProject)
            : base(modelProject)
        {
        }

        /// <summary>
        ///     Resolver method that handles the required internal changes if the space group parameter is replaced
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport ResolveSpaceGroupChange(SpaceGroupInfo groupInfo, IDataAccessor<StructureModelData> dataAccess)
        {
            Console.WriteLine($"Resolver {typeof(SpaceGroupChangeHandler)} was called for {groupInfo.GetType()}");
            return new SpaceGroupChangeHandler(dataAccess, ModelProject).HandleConflicts(groupInfo);
        }

        /// <summary>
        ///     Resolver method that handles the required internal changes if the cell parameters change
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport ResolveCellParametersChange(CellParameters parameters, IDataAccessor<StructureModelData> dataAccess)
        {
            Console.WriteLine($"Resolver {typeof(CellParametersChangeHandler)} was called for {parameters.GetType()}");
            return new CellParametersChangeHandler(dataAccess, ModelProject).HandleConflicts(parameters);
        }
    }
}