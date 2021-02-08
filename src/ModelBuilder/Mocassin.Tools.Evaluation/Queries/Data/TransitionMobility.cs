using System;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Tools.Evaluation.Queries.Data
{
    /// <summary>
    ///     Stores the transition resolved mobility data for a <see cref="IGlobalTrackerModel" />
    /// </summary>
    public class TransitionMobility
    {
        /// <summary>
        ///     Get the <see cref="IGlobalTrackerModel" /> the mobility data belongs to
        /// </summary>
        public IGlobalTrackerModel TrackerModel { get; }

        /// <summary>
        ///     Get the <see cref="EnsembleMobility" /> data
        /// </summary>
        public EnsembleMobility EnsembleMobility { get; }

        /// <summary>
        ///     Creates a new <see cref="TransitionMobility"/> for a <see cref="IGlobalTrackerModel"/>
        /// </summary>
        /// <param name="trackerModel"></param>
        /// <param name="ensembleMobility"></param>
        public TransitionMobility(IGlobalTrackerModel trackerModel, in EnsembleMobility ensembleMobility)
        {
            TrackerModel = trackerModel ?? throw new ArgumentNullException(nameof(trackerModel));
            EnsembleMobility = ensembleMobility;
        }
    }
}