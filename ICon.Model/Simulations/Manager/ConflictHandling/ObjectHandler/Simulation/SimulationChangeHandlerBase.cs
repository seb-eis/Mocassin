﻿using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Operations;
using ICon.Model.Basic;
using ICon.Model.ProjectServices;

namespace ICon.Model.Simulations.ConflictHandling
{
    /// <summary>
    /// Base class for simulation object change handlers. Handles base data conflicts of simulation objects with internal simulation data or
    /// within the object itself
    /// </summary>
    public abstract class SimulationChangeHandlerBase<T> : ObjectConflictHandler<T, SimulationModelData> where T : SimulationBase
    {
        /// <summary>
        /// Create new simulation object change hanlder that uses the provided data accessor and project services
        /// </summary>
        /// <param name="dataAccess"></param>
        /// <param name="projectServices"></param>
        protected SimulationChangeHandlerBase(IDataAccessor<SimulationModelData> dataAccess, IProjectServices projectServices)
            : base(dataAccess, projectServices)
        {

        }

        /// <summary>
        /// Handles internal data conflicts with existing data or within the simulation object itself and creates a conflict report about the actions
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override ConflictReport HandleConflicts(T obj)
        {
            var report = new ConflictReport();
            HandleUndefinedOrEmptyRngSeed(obj, report);
            return report;
        }

        /// <summary>
        /// Handles a potentially undefined RNG seed string and replace it with a GUID
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        public void HandleUndefinedOrEmptyRngSeed(SimulationBase simulation, ConflictReport report)
        {
            if (string.IsNullOrEmpty(simulation.CustomRngSeed))
            {
                simulation.CustomRngSeed = Guid.NewGuid().ToString();
                var detail0 = $"Empty random number generator seed replaced by a GUID value ({simulation.CustomRngSeed})";
                var detail1 = $"Note: GUID values do not require to be random so multiple seed values could potentially be nearly identical";
                report.AddWarning(ModelMessageSource.CreateConflictHandlingWarning(this, detail0, detail1));
            }
        }
    }
}
