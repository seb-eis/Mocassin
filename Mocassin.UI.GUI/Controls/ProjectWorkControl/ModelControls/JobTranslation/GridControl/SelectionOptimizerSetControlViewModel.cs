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
    ///     creation of <see cref="SelectionOptimizerData" /> instances
    /// </summary>
    public sealed class SelectionOptimizerSetControlViewModel : CollectionControlViewModel<SelectionOptimizerData>,
        IContentSupplier<MocassinProject>
    {
        /// <summary>
        ///     Get the <see cref="JobPackageData" /> that hosts the optimizer list
        /// </summary>
        private JobPackageData ParentJobPackageDescription { get; }

        /// <inheritdoc />
        public MocassinProject ContentSource { get; set; }

        /// <summary>
        ///     Get the sequence <see cref="ModelObjectReference{T}" /> instances of <see cref="Particle" /> that are
        ///     selectable in the current state of the object
        /// </summary>
        public IEnumerable<ModelObjectReference<Particle>> SelectableParticles =>
            GetSelectableParticleReferences(SelectedItem);

        /// <summary>
        ///     Get the sequence of <see cref="ModelObjectReference{T}" /> instances of <see cref="CellSite" /> that
        ///     are selectable in the current state of the object
        /// </summary>
        public IEnumerable<ModelObjectReference<CellSite>> SelectablePositions =>
            GetSelectableCellReferencePositionReferences(SelectedItem);

        /// <summary>
        ///     Creates new <see cref="SelectionOptimizerSetControlViewModel" />
        /// </summary>
        /// <param name="parentJobPackageDescription"></param>
        /// <param name="contentSource"></param>
        public SelectionOptimizerSetControlViewModel(JobPackageData parentJobPackageDescription,
            MocassinProject contentSource)
        {
            ParentJobPackageDescription =
                parentJobPackageDescription ?? throw new ArgumentNullException(nameof(parentJobPackageDescription));
            ChangeContentSource(contentSource);
            SetCollection(ParentJobPackageDescription.SelectionOptimizers);
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            SelectedItem = null;
        }

        /// <summary>
        ///     Get the sequence of <see cref="ModelObjectReference{T}" /> instances of <see cref="Particle" /> that
        ///     are selectable in the current state of the object
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public IEnumerable<ModelObjectReference<Particle>> GetSelectableParticleReferences(SelectionOptimizerData current)
        {
            var baseCollection = ContentSource?.ProjectModelData?.ParticleModelData?.Particles;
            return baseCollection?.Select(x => new ModelObjectReference<Particle>(x));
        }

        /// <summary>
        ///     Get the sequence of <see cref="ModelObjectReference{T}" /> instances of <see cref="CellSite" /> that
        ///     are selectable in the current state of the object
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public IEnumerable<ModelObjectReference<CellSite>> GetSelectableCellReferencePositionReferences(
            SelectionOptimizerData current)
        {
            var baseCollection = ContentSource?.ProjectModelData?.StructureModelData?.CellReferencePositions;
            return baseCollection
                   ?.Where(x => x.Stability == PositionStability.Stable)
                   .Select(x => new ModelObjectReference<CellSite>(x));
        }
    }
}