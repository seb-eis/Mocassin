using System;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    /// <summary>
    ///     Struct that contains the <see cref="EnsembleDisplacement" /> of a <see cref="IGlobalTrackerModel" />
    /// </summary>
    public class GlobalTrackerResult
    {
        /// <summary>
        ///     Get the <see cref="IGlobalTrackerModel" /> that the data belongs to
        /// </summary>
        public IGlobalTrackerModel TrackerModel { get; }

        /// <summary>
        ///     Get the <see cref="EnsembleDisplacement" /> that belongs to the tracker
        /// </summary>
        public EnsembleDisplacement DisplacementData { get; }

        /// <summary>
        ///     Creates a new <see cref="GlobalTrackerResult"/> for an <see cref="IGlobalTrackerModel"/>
        /// </summary>
        /// <param name="trackerModel"></param>
        /// <param name="displacementData"></param>
        public GlobalTrackerResult(IGlobalTrackerModel trackerModel, in EnsembleDisplacement displacementData)
        {
            TrackerModel = trackerModel ?? throw new ArgumentNullException(nameof(trackerModel));
            DisplacementData = displacementData;
        }

        /// <inheritdoc />
        public override string ToString() =>
            $"[{TrackerModel.KineticTransitionModel.Transition.Name}]:[{DisplacementData.Particle.Name}][{DisplacementData.VectorR}]";
    }
}