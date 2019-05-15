using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="SelectionOptimizerSetControlView" /> that controls
    ///     creation of <see cref="SelectionOptimizerGraph" /> instances
    /// </summary>
    public sealed class SelectionOptimizerSetControlViewModel : CollectionControlViewModel<SelectionOptimizerGraph>,
        IContentSupplier<MocassinProjectGraph>
    {
        /// <summary>
        ///     Get the <see cref="JobPackageDescriptionGraph" /> that hosts the optimizer list
        /// </summary>
        private JobPackageDescriptionGraph ParentJobPackageDescription { get; }

        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; set; }

        /// <summary>
        ///     Get the sequence <see cref="ModelObjectReferenceGraph{T}" /> instances of <see cref="Particle" /> that are
        ///     selectable in the current state of the object
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<Particle>> SelectableParticles =>
            GetSelectableParticleReferences(SelectedCollectionItem);

        /// <summary>
        ///     Get the sequence of <see cref="ModelObjectReferenceGraph{T}" /> instances of <see cref="UnitCellPosition" /> that
        ///     are selectable in the current state of the object
        /// </summary>
        public IEnumerable<ModelObjectReferenceGraph<UnitCellPosition>> SelectablePositions =>
            GetSelectableUnitCellPositionReferences(SelectedCollectionItem);

        /// <summary>
        ///     Creates new <see cref="SelectionOptimizerSetControlViewModel" />
        /// </summary>
        /// <param name="parentJobPackageDescription"></param>
        /// <param name="contentSource"></param>
        public SelectionOptimizerSetControlViewModel(JobPackageDescriptionGraph parentJobPackageDescription,
            MocassinProjectGraph contentSource)
        {
            ParentJobPackageDescription =
                parentJobPackageDescription ?? throw new ArgumentNullException(nameof(parentJobPackageDescription));
            ChangeContentSource(contentSource);
            SetCollection(ParentJobPackageDescription.SelectionOptimizers);
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            SelectedCollectionItem = null;
        }

        /// <summary>
        ///     Get the sequence of <see cref="ModelObjectReferenceGraph{T}" /> instances of <see cref="Particle" /> that
        ///     are selectable in the current state of the object
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public IEnumerable<ModelObjectReferenceGraph<Particle>> GetSelectableParticleReferences(SelectionOptimizerGraph current)
        {
            // ToDo: Change filter to remove duplicates and redundant definitions
            var baseCollection = ContentSource?.ProjectModelGraph?.ParticleModelGraph?.Particles;
            return baseCollection?.Select(x => new ModelObjectReferenceGraph<Particle>(x));
        }

        /// <summary>
        ///     Get the sequence of <see cref="ModelObjectReferenceGraph{T}" /> instances of <see cref="UnitCellPosition" /> that
        ///     are selectable in the current state of the object
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public IEnumerable<ModelObjectReferenceGraph<UnitCellPosition>> GetSelectableUnitCellPositionReferences(
            SelectionOptimizerGraph current)
        {
            // ToDo: Change filter to remove duplicates and redundant definitions
            var baseCollection = ContentSource?.ProjectModelGraph?.StructureModelGraph?.UnitCellPositions;
            return baseCollection?.Select(x => new ModelObjectReferenceGraph<UnitCellPosition>(x));
        }
    }
}