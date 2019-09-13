using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Framework.Extensions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;
using Mocassin.UI.Xml.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for controlling sets of <see cref="UnitCellPositionGraph" /> of a
    ///     selectable <see cref="MocassinProjectGraph" />
    /// </summary>
    public class CellPositionControlViewModel : CollectionControlViewModel<UnitCellPositionGraph>,
        IContentSupplier<MocassinProjectGraph>
    {
        private IList<Fractional3D> selectedVectorExpansion;

        /// <summary>
        ///     Get the <see cref="ParameterControlViewModel"/> that controls the space group
        /// </summary>
        private StructureParameterControlViewModel ParameterControlViewModel { get; }

        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Get the number of vectors in the currently selected vector expansion
        /// </summary>
        public int SelectedVectorExpansionCount => selectedVectorExpansion?.Count ?? 0;

        /// <summary>
        ///     Get or set a <see cref="IList{T}"/> of the expanded <see cref="Fractional3D"/> of the current graph selection
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

        /// <inheritdoc />
        public CellPositionControlViewModel(StructureParameterControlViewModel parameterControlViewModel)
        {
            ParameterControlViewModel = parameterControlViewModel 
                                        ?? throw new ArgumentNullException(nameof(parameterControlViewModel));
            PropertyChanged += UpdateVectorsOnSelectionChange;
        }

        /// <summary>
        ///     Get the <see cref="IEnumerable{T}"/> of possible occupation <see cref="ParticleSetGraph"/>
        /// </summary>
        public IEnumerable<ParticleSetGraph> OccupationSetOptions => ContentSource?.ProjectModelGraph?.ParticleModelGraph?.ParticleSets;

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            var modelGraph = contentSource?.ProjectModelGraph?.StructureModelGraph;
            Items = modelGraph?.UnitCellPositions;
        }

        /// <summary>
        ///     Get the expanded vector <see cref="IList{T}"/> of the passed <see cref="positionGraph"/>
        /// </summary>
        /// <param name="positionGraph"></param>
        /// <returns></returns>
        public IList<Fractional3D> GetExpandedVectorCollection(UnitCellPositionGraph positionGraph)
        {
            if (positionGraph == null) return new List<Fractional3D>();
            
            var vector = new Fractional3D(positionGraph.A,positionGraph.B,positionGraph.C);
            return ParameterControlViewModel.SpaceGroupService.GetUnitCellP1PositionExtension(vector);
        }

        /// <summary>
        ///     Action that is called if the selected <see cref="UnitCellPositionGraph"/> changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void UpdateVectorsOnSelectionChange(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(SelectedItem)) return;
            SelectedVectorExpansion = GetExpandedVectorCollection(SelectedItem);
        }
    }
}