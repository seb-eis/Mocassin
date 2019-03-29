using System;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.EnergyModel.DataControl;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.EnergyModel
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="EnergyModelControlView" /> that controls the energy model
    ///     definition
    /// </summary>
    public class EnergyModelControlViewModel : PrimaryControlViewModel, IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Get the <see cref="EnergyParameterControlViewModel"/> that controls the stable environment settings
        /// </summary>
        public EnergyParameterControlViewModel ParameterControlViewModel { get; }

        /// <inheritdoc />
        public EnergyModelControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ParameterControlViewModel = new EnergyParameterControlViewModel();
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
        }
    }
}