using System;
using System.Collections.Generic;
using System.Text;

using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Energies.ConflictHandling
{
    /// <summary>
    /// Conflict handler for parameter change induced conflicts within the energy manager
    /// </summary>
    public class EnergyParameterChangeHandler : DataConflictHandler<EnergyModelData, ModelParameter>
    {
        /// <summary>
        /// Construct new energy parameter conflict resolver that uses the provided project services
        /// </summary>
        /// <param name="projectServices"></param>
        public EnergyParameterChangeHandler(IProjectServices projectServices) : base(projectServices)
        {

        }

        /// <summary>
        /// Resolver method that handles the required internal changes if the stabel environment info changes
        /// </summary>
        /// <param name="info"></param>
        /// <param name="dataAccess"></param>
        /// <returns></returns>
        [ConflictHandlingMethod(DataOperationType.ParameterChange)]
        protected IConflictReport ResolveParameterChange(StableEnvironmentInfo info, IDataAccessor<EnergyModelData> dataAccess)
        {
            Console.WriteLine($"Resolver {typeof(StableEnvironmentInfoChangeHandler)} was called for {info.GetType().ToString()}");
            return new StableEnvironmentInfoChangeHandler().Resolve(info, dataAccess, ProjectServices);
        }
    }
}
