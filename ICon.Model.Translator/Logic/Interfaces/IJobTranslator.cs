﻿using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Simulations;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Represents a translator for MC simulation models into
    /// </summary>
    public interface IJobTranslator
    {
        ITranslationContext TranslationContext { get; set; }

        JobModel Translate<TSimulation>(TSimulation simulation) where TSimulation : ISimulation;

        IEnumerable<JobModel> TranslateSeries<TSeries>(TSeries simulationSeries) where TSeries : ISimulationSeries;
    }
}
