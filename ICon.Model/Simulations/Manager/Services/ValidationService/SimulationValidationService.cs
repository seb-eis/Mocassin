using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    /// Implementation of the validation service for simulation manager related model objects and parameters
    /// </summary>
    public class SimulationValidationService : ValidationService<ISimulationDataPort>
    {
        /// <summary>
        /// The simulation settings used in the validation methods
        /// </summary>
        protected MocassinSimulationSettings Settings { get; }

        /// <inheritdoc />
        public SimulationValidationService(MocassinSimulationSettings settings, IModelProject modelProject)
            : base(modelProject)
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
            return new KineticSimulationValidator(ModelProject, Settings, dataReader).Validate(simulation);
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
            return new MetropolisSimulationValidator(ModelProject, Settings, dataReader).Validate(simulation);
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
            return new KineticSeriesValidator(ModelProject, Settings, dataReader).Validate(series);
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
            return new MetropolisSeriesValidator(ModelProject, Settings, dataReader).Validate(series);
        }
    }
}
