using System;
using Mocassin.Framework.Operations;
using Mocassin.Model.Basic;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Simulations.ConflictHandling
{
    /// <summary>
    ///     Base class for simulation object change handlers. Handles base data conflicts of simulation objects with internal
    ///     simulation data or
    ///     within the object itself
    /// </summary>
    public abstract class SimulationChangeHandlerBase<T> : ObjectConflictHandler<T, SimulationModelData>
        where T : SimulationBase
    {
        /// <inheritdoc />
        protected SimulationChangeHandlerBase(IDataAccessor<SimulationModelData> dataAccessor, IModelProject modelProject)
            : base(dataAccessor, modelProject)
        {
        }

        /// <inheritdoc />
        public override ConflictReport HandleConflicts(T obj)
        {
            var report = new ConflictReport();
            HandleUndefinedOrEmptyRngSeed(obj, report);
            return report;
        }

        /// <summary>
        ///     Handles a potentially undefined RNG seed string and replace it with a GUID
        /// </summary>
        /// <param name="simulation"></param>
        /// <param name="report"></param>
        public void HandleUndefinedOrEmptyRngSeed(SimulationBase simulation, ConflictReport report)
        {
            if (!string.IsNullOrEmpty(simulation.CustomRngSeed))
                return;

            simulation.CustomRngSeed = Guid.NewGuid().ToString();
            var detail0 = $"Empty random number generator seed replaced by a GUID value ({simulation.CustomRngSeed})";
            const string detail1 = "Note: GUID values do not require to be random so multiple seed values could potentially be nearly identical";
            report.AddWarning(ModelMessageSource.CreateConflictHandlingWarning(this, detail0, detail1));
        }
    }
}