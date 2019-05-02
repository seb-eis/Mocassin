using System.Linq;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="GroupInteractionControlView" /> that controls
    ///     <see cref="GroupEnergySetGraph" /> customization
    /// </summary>
    public class GroupInteractionControlViewModel : CollectionControlViewModel<GroupEnergySetGraph>,
        IContentSupplier<ProjectCustomizationGraph>
    {
        /// <inheritdoc />
        public ProjectCustomizationGraph ContentSource { get; protected set; }

        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            if (contentSource is ProjectCustomizationGraph customizationGraph) ChangeContentSource(customizationGraph);
        }

        /// <inheritdoc />
        public void ChangeContentSource(ProjectCustomizationGraph contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.EnergyModelCustomization?.GroupEnergyParameterSets);
        }
    }
}