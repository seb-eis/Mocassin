using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Energies
{
    /// <inheritdoc cref="IUnstableEnvironment" />
    public class UnstableEnvironment : ModelObject, IUnstableEnvironment
    {
        /// <inheritdoc />
        public double MaxInteractionRange { get; set; }

        /// <inheritdoc />
        [UseTrackedData]
        public ICellReferencePosition CellReferencePosition { get; set; }

        /// <summary>
        ///     The set of defined interaction filters of the unstable environment (Can be empty)
        /// </summary>
        [UseTrackedData(ReferenceCorrectionLevel = ReferenceCorrectionLevel.IgnoreTopLevel)]
        public List<AsymmetricInteractionFilter> InteractionFilters { get; set; }

        /// <summary>
        ///     The list of generated pair interactions (Can be null, automatically managed and linked property, not part of object
        ///     population)
        /// </summary>
        [UseTrackedData]
        public List<IAsymmetricPairInteraction> PairInteractions { get; set; }

        /// <summary>
        ///     The list of generated group interactions (Can be null, automatically managed and linked property, not part of
        ///     object
        ///     population)
        /// </summary>
        [UseTrackedData]
        public List<IGroupInteraction> GroupInteractions { get; set; }

        /// <summary>
        ///     Create new unstable environment and sets all lists to empty
        /// </summary>
        public UnstableEnvironment()
        {
            InteractionFilters = new List<AsymmetricInteractionFilter>();
            PairInteractions = new List<IAsymmetricPairInteraction>();
            GroupInteractions = new List<IGroupInteraction>();
        }

        /// <inheritdoc />
        public IEnumerable<IInteractionFilter> GetInteractionFilters()
        {
            return (InteractionFilters ?? new List<AsymmetricInteractionFilter>()).AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<IAsymmetricPairInteraction> GetPairInteractions()
        {
            return (PairInteractions ?? new List<IAsymmetricPairInteraction>()).AsEnumerable();
        }

        /// <inheritdoc />
        public IEnumerable<IGroupInteraction> GetGroupInteractions()
        {
            return (GroupInteractions ?? new List<IGroupInteraction>()).AsEnumerable();
        }

        /// <inheritdoc />
        public override string ObjectName => "Unstable Environment Info";

        /// <inheritdoc />
        public override ModelObject PopulateFrom(IModelObject obj)
        {
            if (!(CastIfNotDeprecated<IUnstableEnvironment>(obj) is { } info))
                return null;

            CellReferencePosition = info.CellReferencePosition;
            MaxInteractionRange = info.MaxInteractionRange;
            InteractionFilters = info.GetInteractionFilters().Select(AsymmetricInteractionFilter.FromInterface).ToList();
            GroupInteractions = info.GetGroupInteractions().ToList();
            return this;
        }
    }
}