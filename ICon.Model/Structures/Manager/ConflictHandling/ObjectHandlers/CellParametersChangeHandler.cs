using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Structures.ConflictHandling
{
    /// <summary>
    ///     Resolves conflicts that are affiliated with the change of the unit cell parameters
    /// </summary>
    public class CellParametersChangeHandler : ObjectConflictHandler<CellParameters, StructureModelData>
    {
        /// <inheritdoc />
        public CellParametersChangeHandler(IDataAccessor<StructureModelData> dataAccessor, IModelProject modelProject)
            : base(dataAccessor, modelProject)
        {
        }

        /// <inheritdoc />
        public override ConflictReport HandleConflicts(CellParameters parameters)
        {
            var report = new ConflictReport();
            MatchParameterSetsWithCrystalSystem(parameters, report);
            return report;
        }

        /// <summary>
        ///     Enforces consistency of the parameter set between the crystal system and the cell parameter input
        ///     (This methods throw if the parameter set is rejected by the crystal system service)
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="report"></param>
        protected void MatchParameterSetsWithCrystalSystem(CellParameters parameters, ConflictReport report)
        {
            if (ModelProject.CrystalSystemService.TrySetParameters(parameters.AsParameterSet()))
            {
                parameters.ParameterSet = ModelProject.CrystalSystemService.CopyCurrentParameterSet();
                var detail0 = $"The change in the {parameters.GetParameterName()} was passed to the crystal system provider system";
                report.Warnings.Add(ModelMessageSource.CreateConflictHandlingWarning(this, detail0));
            }
            else
                throw new InvalidOperationException("Unsupported cell parameter set passed affiliated validations");
        }
    }
}