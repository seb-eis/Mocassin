﻿using System;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    /// <summary>
    ///     Stores the transition resolved mobility data for a <see cref="IGlobalTrackerModel" />
    /// </summary>
    public readonly struct TransitionMobility
    {
        /// <summary>
        ///     Get the <see cref="IGlobalTrackerModel" /> the mobility data belongs to
        /// </summary>
        public IGlobalTrackerModel TrackerModel { get; }

        /// <summary>
        ///     Get the <see cref="EnsembleMobility" /> data
        /// </summary>
        public EnsembleMobility EnsembleMobility { get; }

        /// <inheritdoc />
        public TransitionMobility(IGlobalTrackerModel trackerModel, in EnsembleMobility ensembleMobility)
            : this()
        {
            TrackerModel = trackerModel ?? throw new ArgumentNullException(nameof(trackerModel));
            EnsembleMobility = ensembleMobility;
        }
    }
}