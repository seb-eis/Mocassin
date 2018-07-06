﻿using System;
using System.Collections.Generic;
using System.Text;
using ICon.Framework.Constraints;

namespace ICon.Model.Simulations
{
    /// <summary>
    /// Represents a specialized simulation sereis for kinetic monte carlo simulations
    /// </summary>
    public interface IKineticSimulationSeries : ISimulationSeries
    {
        /// <summary>
        /// Get the kinetic base simulation of the series
        /// </summary>
        new IKineticSimulation BaseSimulation { get; }

        /// <summary>
        /// Get the series for the electric field magnitude values
        /// </summary>
        IValueSeries ElectricFieldSeries { get; }

        /// <summary>
        /// Get the series for the dynamic normalization probability values
        /// </summary>
        IValueSeries NormalizationProbabilitySeries { get; }
    }
}
