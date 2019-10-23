using System;
using System.Linq;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for the<see cref="PairEnergySetControlView" />
    /// </summary>
    public sealed class PairEnergySetControlViewModel : CollectionControlViewModel<PairEnergyGraph>
    {
        /// <summary>
        ///     Get the <see cref="PairInteractionGraph" /> that the view model targets
        /// </summary>
        public PairInteractionGraph PairInteraction { get; }

        /// <summary>
        ///     Create new <see cref="PairEnergySetControlViewModel" />
        /// </summary>
        /// <param name="pairInteraction"></param>
        public PairEnergySetControlViewModel(PairInteractionGraph pairInteraction)
        {
            PairInteraction = pairInteraction ?? throw new ArgumentNullException(nameof(pairInteraction));
            SetCollection(pairInteraction.PairEnergyEntries);
            SelectedItem = Items?.FirstOrDefault();
        }
    }
}