﻿using System;
using ICon.Model.Basic;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Represents a query port for a simulation manager that provides save query access to the simulation model data and cache objects
    /// </summary>
    public interface ISimulationQueryPort : IModelQueryPort<ISimulationDataPort>, IModelQueryPort<ISimulationCachePort>
    {

    }
}
