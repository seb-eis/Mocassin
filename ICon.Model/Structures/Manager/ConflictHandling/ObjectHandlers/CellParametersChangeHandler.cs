﻿using System;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Structures.ConflictHandling
{
    /// <summary>
    /// Resolves conflichts that are affiliated with the change of the unit cell parameters
    /// </summary>
    public class CellParametersChangeHandler : ObjectConflictHandler<CellParameters, StructureModelData>
    {
        /// <summary>
        /// Create new cell parameter changed handler with the provided data acessor and project services
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        public CellParametersChangeHandler(IDataAccessor<StructureModelData> dataAccess, IProjectServices projectServices)
            : base(dataAccess, projectServices)
        {
        }

        /// <summary>
        /// Resolves the conflichts caused by a change in the unit cell parameters (Update the crystal system service)
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(CellParameters parameters)
        {
            var report = new ConflictReport();
            MatchParameterSetsWithCrystalSystem(parameters, report);
            return report;
        }

        /// <summary>
        /// Enforces consistency of the parameter set between the cyrstal system and the cell parameter input
        /// (This methods throw if the parameter set is rejected by the cyrstal system service)
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="report"></param>
        protected void MatchParameterSetsWithCrystalSystem(CellParameters parameters, ConflictReport report)
        {
            if (ProjectServices.CrystalSystemService.TrySetParameters(parameters.AsParameterSet()))
            {
                parameters.ParameterSet = ProjectServices.CrystalSystemService.GetCurrentParameterSet();
                var detail0 = $"The change in the {parameters.GetParameterName()} was passed to the crystal system provider system";
                report.Warnings.Add(ModelMessages.CreateConflictHandlingWarning(this, detail0));
            }
            else
            {
                throw new InvalidOperationException("Unsupported cell parameter set passed affiliated validations");
            }
        }
    }
}
