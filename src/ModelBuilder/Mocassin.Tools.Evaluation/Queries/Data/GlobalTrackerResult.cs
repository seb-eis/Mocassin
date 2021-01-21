using System;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Tools.Evaluation.Queries.Data
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

        /// <inheritdoc />
        public GlobalTrackerResult(IGlobalTrackerModel trackerModel, in EnsembleDisplacement displacementData)
            : this()
        {
            TrackerModel = trackerModel ?? throw new ArgumentNullException(nameof(trackerModel));
            DisplacementData = displacementData;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"[{TrackerModel.KineticTransitionModel.Transition.Name}]:[{DisplacementData.Particle.Name}][{DisplacementData.VectorR}]";
    }
}