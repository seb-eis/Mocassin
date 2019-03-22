using System;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.StructureModel.DataControl
{
    /// <summary>
    ///     The <see cref="DataCollectionControlViewModel{T}" /> for controlling sets of <see cref="UnitCellPositionGraph" /> of a
    ///     selectable <see cref="MocassinProjectGraph" />
    /// </summary>
    public class CellPositionControlViewModel : DataCollectionControlViewModel<UnitCellPositionGraph>,
        IContentSupplier<MocassinProjectGraph>
    {
        /// <summary>
        ///     Get the <see cref="ParameterControlViewModel"/> that controls the space group
        /// </summary>
        private StructureParameterControlViewModel ParameterControlViewModel { get; }

        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <inheritdoc />
        public CellPositionControlViewModel(StructureParameterControlViewModel parameterControlViewModel)
        {
            ParameterControlViewModel = parameterControlViewModel 
                                        ?? throw new ArgumentNullException(nameof(parameterControlViewModel));
        }


        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            if (contentSource is MocassinProjectGraph projectGraph) ChangeContentSource(projectGraph);
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            var modelGraph = contentSource?.ProjectModelGraph?.StructureModelGraph;
            DataCollection = modelGraph?.UnitCellPositions;
        }
    }
}