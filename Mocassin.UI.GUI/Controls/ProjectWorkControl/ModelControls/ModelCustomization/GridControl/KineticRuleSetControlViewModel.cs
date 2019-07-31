using System;
using System.Linq;
using Mocassin.UI.Base.Commands.UiCommands;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.GridControl
{
    /// <summary>
    ///     The <see cref="ViewModelBase"/> for the <see cref="KineticRuleSetControlView"/> that enables customization of
    /// </summary>
    public sealed class KineticRuleSetControlViewModel : CollectionControlViewModel<KineticRuleGraph>
    {
        /// <summary>
        ///     Get the <see cref="KineticRuleSetGraph"/> that the view model targets
        /// </summary>
        public KineticRuleSetGraph KineticRuleSet { get; }

        /// <summary>
        ///     Create new <see cref="KineticRuleSetControlViewModel"/> for the passed <see cref="KineticRuleGraph"/>
        /// </summary>
        /// <param name="kineticRuleSet"></param>
        public KineticRuleSetControlViewModel(KineticRuleSetGraph kineticRuleSet)
        {
            KineticRuleSet = kineticRuleSet ?? throw new ArgumentNullException(nameof(kineticRuleSet));
            SetCollection(kineticRuleSet.KineticRules);
            SelectedItem = Items?.FirstOrDefault();
        }
    }
}