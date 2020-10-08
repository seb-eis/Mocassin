using System.Collections.ObjectModel;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.GridControl;
using Mocassin.UI.Data.Customization;

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
            UpdateSetControlViewModels();
        }

        /// <summary>
        ///     Updates the required <see cref="GroupEnergySetControlViewModel" /> collection
        /// </summary>
        private void UpdateSetControlViewModels()
        {
            Items ??= new ObservableCollection<GroupEnergySetControlViewModel>();
            Items.Clear();

            var interactionSets = ContentSource?.EnergyModelCustomization?.GroupEnergyParameterSets;
            if (interactionSets == null) return;

            Items.AddRange(interactionSets.Select(groupEnergySetData => new GroupEnergySetControlViewModel(groupEnergySetData)));
        }
    }
}