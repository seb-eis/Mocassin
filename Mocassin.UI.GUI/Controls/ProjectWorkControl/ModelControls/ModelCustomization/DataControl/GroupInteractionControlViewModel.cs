using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.GridControl;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="GroupInteractionControlView" /> that controls
    ///     <see cref="GroupEnergySetControlViewModel" /> customization
    /// </summary>
    public class GroupInteractionControlViewModel : CollectionControlViewModel<GroupEnergySetControlViewModel>,
        IContentSupplier<ProjectCustomizationTemplate>
    {
        /// <inheritdoc />
        public ProjectCustomizationTemplate ContentSource { get; protected set; }

        /// <inheritdoc />
        public void ChangeContentSource(ProjectCustomizationTemplate contentSource)
        {
            ContentSource = contentSource;
            CreateSetControlViewModels();
        }

        /// <summary>
        ///     Creates and sets the new <see cref="GroupEnergySetControlViewModel" /> collection
        /// </summary>
        private void CreateSetControlViewModels()
        {
            var interactionSets = ContentSource?.EnergyModelCustomization?.GroupEnergyParameterSets;
            if (interactionSets == null)
            {
                SetCollection(null);
                return;
            }

            var viewModels = interactionSets.Select(x => new GroupEnergySetControlViewModel(x)).ToList(interactionSets.Count);
            SetCollection(viewModels);
        }
    }
}