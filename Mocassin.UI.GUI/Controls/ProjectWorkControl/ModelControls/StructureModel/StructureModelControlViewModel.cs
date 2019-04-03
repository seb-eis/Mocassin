using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel.DataControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel"/> that controls <see cref="StructureModelControlView"/>
    /// </summary>
    public class StructureModelControlViewModel : PrimaryControlViewModel, IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Get the <see cref="CellPositionControlViewModel"/> that controls wyckoff positions
        /// </summary>
        public CellPositionControlViewModel PositionControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="StructureParameterControlViewModel"/> that controls geometry and misc info
        /// </summary>
        public StructureParameterControlViewModel ParameterControlViewModel { get; }

        /// <inheritdoc />
        public StructureModelControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ParameterControlViewModel = new StructureParameterControlViewModel(projectControl);
            PositionControlViewModel = new CellPositionControlViewModel(ParameterControlViewModel);
        }

        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            if (contentSource is MocassinProjectGraph projectGraph) ChangeContentSource(projectGraph);
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ParameterControlViewModel.ChangeContentSource(contentSource);
            PositionControlViewModel.ChangeContentSource(contentSource);
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ChangeContentSource(null);
        }
    }
}