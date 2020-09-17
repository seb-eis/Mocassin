using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;
using Mocassin.UI.Xml.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for controlling sets of <see cref="CellReferencePositionData" />
    ///     of a
    ///     selectable <see cref="MocassinProject" />
    /// </summary>
    public class CellPositionControlViewModel : CollectionControlViewModel<CellReferencePositionData>,
        IContentSupplier<MocassinProject>
    {
        private IList<Fractional3D> selectedVectorExpansion;

        /// <summary>
        ///     Get the <see cref="ParameterControlViewModel" /> that controls the space group
        /// </summary>
        private StructureParameterControlViewModel ParameterControlViewModel { get; }

        /// <inheritdoc />
        public MocassinProject ContentSource { get; protected set; }

        /// <summary>
        ///     Get the number of vectors in the currently selected vector expansion
        /// </summary>
        public int SelectedVectorExpansionCount => selectedVectorExpansion?.Count ?? 0;

        /// <summary>
        ///     Get or set a <see cref="IList{T}" /> of the expanded <see cref="Fractional3D" /> of the current graph selection
        /// </summary>
        public IList<Fractional3D> SelectedVectorExpansion
        {
            get => selectedVectorExpansion;
            private set
            {
                SetProperty(ref selectedVectorExpansion, value);
                OnPropertyChanged(nameof(SelectedVectorExpansionCount));
            }
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of possible occupation <see cref="ParticleSetData" />
        /// </summary>
        public IEnumerable<ModelObjectReference<ParticleSet>> OccupationSetOptions => EnumerateOccupationSetOptions();

        /// <inheritdoc />
        public CellPositionControlViewModel(StructureParameterControlViewModel parameterControlViewModel)
        {
            ParameterControlViewModel = parameterControlViewModel
                                        ?? throw new ArgumentNullException(nameof(parameterControlViewModel));
            PropertyChanged += UpdateVectorsOnSelectionChange;
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            var modelGraph = contentSource?.ProjectModelData?.StructureModelData;
            Items = modelGraph?.CellReferencePositions;
        }

        /// <summary>
        ///     Get the expanded vector <see cref="IList{T}" /> of the passed <see cref="CellReferencePositionData" />
        /// </summary>
        /// <param name="referencePositionData"></param>
        /// <returns></returns>
        public IList<Fractional3D> GetExpandedVectorCollection(CellReferencePositionData referencePositionData)
        {
            if (referencePositionData == null) return new List<Fractional3D>();

            var vector = new Fractional3D(referencePositionData.A, referencePositionData.B, referencePositionData.C);
            return ParameterControlViewModel.SpaceGroupService.GetUnitCellP1PositionExtension(vector);
        }

        /// <summary>
        ///     Action that is called if the selected <see cref="CellReferencePositionData" /> changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void UpdateVectorsOnSelectionChange(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(SelectedItem)) return;
            SelectedVectorExpansion = GetExpandedVectorCollection(SelectedItem);
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}" /> of selectable <see cref="ModelObjectReference{T}" /> for
        ///     <see cref="ParticleSet" /> model objects
        /// </summary>
        /// <returns></returns>
        private IEnumerable<ModelObjectReference<ParticleSet>> EnumerateOccupationSetOptions()
        {
            return ContentSource?.ProjectModelData?.ParticleModelData?.ParticleSets.Select(x => new ModelObjectReference<ParticleSet>(x));
        }
    }
}