using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Energies.ConflictHandling
{
    /// <summary>
    ///     Conflict handler for parameter change induced conflicts within the energy manager
    /// </summary>
    public class EnergyParameterChangeHandler : DataConflictHandler<EnergyModelData, ModelParameter>
    {
        /// <inheritdoc />
        public EnergyParameterChangeHandler(IModelProject modelProject)
            : base(modelProject)
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
            return new StableEnvironmentInfoChangeHandler(dataAccess, ModelProject).HandleConflicts(info);
        }
    }
}