using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Implementation of the validation service for simulation manager related model objects and parameters
    /// </summary>
    public class SimulationValidationService : ValidationService<ISimulationDataPort>
    {
        /// <summary>
        /// The simulation settings used in the validation methods
        /// </summary>
        protected BasicSimulationSettings Settings { get; }

        /// <inheritdoc />
        public SimulationValidationService(BasicSimulationSettings settings, IProjectServices projectServices)
            : base(projectServices)
        {
            Settings = settings;
        }

        /// <summary>
        /// Validates the passed kinetic simulation and returns the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport Validate(IKineticSimulation simulation, IDataReader<ISimulationDataPort> dataReader)
        {
            return new KineticSimulationValidator(ProjectServices, Settings, dataReader).Validate(simulation);
        }

        /// <summary>
        /// Validates the passed metropolis simulation and returns the validation report
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport Validate(IMetropolisSimulation simulation, IDataReader<ISimulationDataPort> dataReader)
        {
            return new MetropolisSimulationValidator(ProjectServices, Settings, dataReader).Validate(simulation);
        }

        /// <summary>
        /// Validates the passed kinetic simulation series and returns the validation report
        /// </summary>
        /// <param name="series"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport Validate(IKineticSimulationSeries series, IDataReader<ISimulationDataPort> dataReader)
        {
            return new KineticSeriesValidator(ProjectServices, Settings, dataReader).Validate(series);
        }

        /// <summary>
        /// Validates the passed metropolis simulation series and returns the validation report
        /// </summary>
        /// <param name="series"></param>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        [ValidationOperation(ValidationType.Object)]
        protected IValidationReport Validate(IMetropolisSimulationSeries series, IDataReader<ISimulationDataPort> dataReader)
        {
            return new MetropolisSeriesValidator(ProjectServices, Settings, dataReader).Validate(series);
        }
    }
}
