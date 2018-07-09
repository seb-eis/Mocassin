using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Abstract base class for implementations of simulation series validators. This class provides the basic shared validation functions
    /// </summary>
    public abstract class SimulationSeriesBaseValidator : DataValidator<ISimulationSeriesBase, BasicSimulationSettings, ISimulationDataPort>
    {
        /// <summary>
        /// Create new simulation series base validator from project services, simulation settings and simulation model data reader
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        /// <param name="dataReader"></param>
        protected SimulationSeriesBaseValidator(IProjectServices projectServices, BasicSimulationSettings settings, IDataReader<ISimulationDataPort> dataReader)
            : base(projectServices, settings, dataReader)
        {

        }

        /// <summary>
        /// Validates a simulation series base data in terms of violations of restrictions or internal data conflicts and returns a validation report
        /// containing the results
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override IValidationReport Validate(ISimulationSeriesBase obj)
        {
            var report = new ValidationReport();



            return report;
        }

        /// <summary>
        /// VAlidates the string property values of the simulation series and adds the results to the validation report
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        protected virtual void AddStringPropertyValidations(ISimulationSeriesBase series, ValidationReport report)
        {

        }

        /// <summary>
        /// Validares all value series objects defined in the simulation series and adds the results to the validation report
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        protected virtual void AddValueSeriesValidations(ISimulationSeriesBase series, ValidationReport report)
        {

        }

        /// <summary>
        /// Validate the doping series object of the simulation series and adds the results to the validation report
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        protected virtual void AddDopingSeriesValidation(ISimulationSeriesBase series, ValidationReport report)
        {

        }

        /// <summary>
        /// Validates loadability of all external data sources of the series and adds the results to the validation report
        /// </summary>
        /// <param name="series"></param>
        /// <param name="report"></param>
        protected virtual void AddExternalDataSourceValidations(ISimulationSeriesBase series, ValidationReport report)
        {

        }
    }
}
