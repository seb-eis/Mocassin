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
        ///     Get the <see cref="PairEnergySetGraph" /> that the view model targets
        /// </summary>
        public PairEnergySetGraph PairEnergySet { get; }

        /// <summary>
        ///     Create new <see cref="PairEnergySetControlViewModel" />
        /// </summary>
        /// <param name="pairEnergySet"></param>
        public PairEnergySetControlViewModel(PairEnergySetGraph pairEnergySet)
        {
            PairEnergySet = pairEnergySet ?? throw new ArgumentNullException(nameof(pairEnergySet));
            SetCollection(pairEnergySet.PairEnergyEntries);
            SelectedItem = Items?.FirstOrDefault();
        }
    }
}