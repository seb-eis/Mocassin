using System;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies.ConflictHandling
{
    /// <summary>
    ///     Conflict handler for parameter change induced conflicts within the energy manager
    /// </summary>
    public class EnergyParameterChangeHandler : DataConflictHandler<EnergyModelData, ModelParameter>
    {
        /// <inheritdoc />
        public EnergyParameterChangeHandler(IProjectServices projectServices)
            : base(projectServices)
        {
        }

        /// <summary>
        ///     Resolver method that handles the required internal changes if the stabel environment info changes
        /// </summary>
        /// <param name="info"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod]
        protected IConflictReport HandleParameterChange(StableEnvironmentInfo info, IDataAccessor<EnergyModelData> dataAccess)
        {
            Console.WriteLine($"Resolver {typeof(StableEnvironmentInfoChangeHandler)} was called for {info.GetType()}");
            return new StableEnvironmentInfoChangeHandler(dataAccess, ProjectServices).HandleConflicts(info);
        }
    }
}