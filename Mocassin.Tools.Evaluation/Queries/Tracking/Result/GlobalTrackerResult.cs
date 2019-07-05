using System;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Tools.Evaluation.Queries
{
    /// <summary>
    ///     Struct that contains the <see cref="EnsembleDisplacement" /> of a <see cref="IGlobalTrackerModel" />
    /// </summary>
    public readonly struct GlobalTrackerResult
    {
        /// <summary>
        ///     Get the <see cref="IGlobalTrackerModel" /> that the data belongs to
        /// </summary>
        public IGlobalTrackerModel TrackerModel { get; }

        /// <summary>
        ///     Get the <see cref="EnsembleDisplacement" /> that belongs to the tracker
        /// </summary>
        public EnsembleDisplacement DisplacementData { get; }

        public GlobalTrackerResult(IGlobalTrackerModel trackerModel, in EnsembleDisplacement displacementData)
            : this()
        {
            TrackerModel = trackerModel ?? throw new ArgumentNullException(nameof(trackerModel));
            DisplacementData = displacementData;
        }
    }
}