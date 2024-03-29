﻿using System;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.TransitionModel
{
    /// <summary>
    ///     Serializable data object for <see cref="Mocassin.Model.Transitions.IMetropolisTransition" /> model object creation
    /// </summary>
    [XmlRoot]
    public class MetropolisTransitionData : ModelDataObject, IEquatable<MetropolisTransitionData>
    {
        private ModelObjectReference<AbstractTransition> abstractTransition = new ModelObjectReference<AbstractTransition>();
        private ModelObjectReference<CellSite> firstCellReferencePosition = new ModelObjectReference<CellSite>();
        private ModelObjectReference<CellSite> secondCellReferencePosition = new ModelObjectReference<CellSite>();

        /// <summary>
        ///     Get or set the abstract transition key for the transition logic
        /// </summary>
        [XmlElement]
        public ModelObjectReference<AbstractTransition> AbstractTransition
        {
            get => abstractTransition;
            set => SetProperty(ref abstractTransition, value);
        }

        /// <summary>
        ///     Get or set the unit cell position key for first involved wyckoff position
        /// </summary>
        [XmlElement]
        public ModelObjectReference<CellSite> FirstCellReferencePosition
        {
            get => firstCellReferencePosition;
            set => SetProperty(ref firstCellReferencePosition, value);
        }

        /// <summary>
        ///     Get or set the unit cell position key for second involved wyckoff position
        /// </summary>
        [XmlElement]
        public ModelObjectReference<CellSite> SecondCellReferencePosition
        {
            get => secondCellReferencePosition;
            set => SetProperty(ref secondCellReferencePosition, value);
        }

        /// <inheritdoc />
        public bool Equals(MetropolisTransitionData other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (AbstractTransition != null && (AbstractTransition == null || !AbstractTransition.Equals(other.AbstractTransition))) return false;
            if (HasNullKeys() || other.HasNullKeys()) return false;
            return SecondCellReferencePosition != null && FirstCellReferencePosition != null &&
                   (FirstCellReferencePosition.Equals(other.FirstCellReferencePosition)
                    && SecondCellReferencePosition.Equals(other.SecondCellReferencePosition)
                    || SecondCellReferencePosition.Equals(other.FirstCellReferencePosition)
                    && FirstCellReferencePosition.Equals(other.SecondCellReferencePosition));
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            var obj = new MetropolisTransition
            {
                AbstractTransition = new AbstractTransition {Key = AbstractTransition.Key},
                FirstCellSite = new CellSite {Key = FirstCellReferencePosition.Key},
                SecondCellSite = new CellSite {Key = SecondCellReferencePosition.Key}
            };
            return obj;
        }

        /// <summary>
        ///     Checks if one of the key <see cref="string" /> values is null
        /// </summary>
        /// <returns></returns>
        public bool HasNullKeys() =>
            AbstractTransition == null
            || FirstCellReferencePosition == null
            || SecondCellReferencePosition == null;
    }
}