using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures.ConflictHandling
{
    /// <summary>
    /// Conflict resolver for parameter induced conflicts within the structure manager
    /// </summary>
    public class StructureParameterConflictHandler : DataConflictHandler<StructureModelData, ModelParameter>
    {
        /// <summary>
        /// Construct new structure parameter conflict resolver that uses the provided project services
        /// </summary>
        /// <param name="projectServices"></param>
        public StructureParameterConflictHandler(IProjectServices projectServices) : base(projectServices)
        {

        }

        /// <summary>
        /// Resolver method that handles the required internal changes if the space group parameter is replaced
        /// </summary>
        /// <param name="groupInfo"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod(DataOperationType.ParameterChange)]
        protected IConflictReport ResolveSpaceGroupChange(SpaceGroupInfo groupInfo, IDataAccessor<StructureModelData> dataAccess)
        {
            Console.WriteLine($"Resolver {typeof(SpaceGroupChangeHandler)} was called for {groupInfo.GetType().ToString()}");
            return new SpaceGroupChangeHandler(dataAccess, ProjectServices).HandleConflicts(groupInfo);
        }

        /// <summary>
        /// Resolver method that handles the required internal changes if the cell parameters change
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod(DataOperationType.ParameterChange)]
        protected IConflictReport ResolveCellParametersChange(CellParameters parameters, IDataAccessor<StructureModelData> dataAccess)
        {
            Console.WriteLine($"Resolver {typeof(CellParametersChangeHandler)} was called for {parameters.GetType().ToString()}");
            return new CellParametersChangeHandler(dataAccess, ProjectServices).HandleConflicts(parameters);
        }
    }
}
