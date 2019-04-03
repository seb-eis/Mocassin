using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="ModelCustomizationControlView" /> that controls model
    ///     translation data customization
    /// </summary>
    public class ModelCustomizationControlViewModel : PrimaryControlViewModel, IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <inheritdoc />
        public ModelCustomizationControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
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