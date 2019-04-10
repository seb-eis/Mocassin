using System;
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
        public UpdateTextBoxSourceCommand UpdateTextBoxSourceCommand { get; }

        /// <summary>
        ///     Create new <see cref="KineticRuleSetControlViewModel"/> for the passed <see cref="KineticRuleGraph"/>
        /// </summary>
        /// <param name="kineticRuleSet"></param>
        public KineticRuleSetControlViewModel(KineticRuleSetGraph kineticRuleSet)
        {
            if (kineticRuleSet == null) throw new ArgumentNullException(nameof(kineticRuleSet));
            UpdateTextBoxSourceCommand = new UpdateTextBoxSourceCommand();
            SetCollection(kineticRuleSet.KineticRules);
        }
    }
}