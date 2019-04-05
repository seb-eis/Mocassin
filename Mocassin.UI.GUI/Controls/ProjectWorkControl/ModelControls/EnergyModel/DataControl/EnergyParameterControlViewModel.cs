using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.GridControl;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.EnergyModel;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="InteractionFilterGraph" /> and radius definitions
    ///     of the stable environment
    /// </summary>
    public class EnergyParameterControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="InteractionFilterGridControlViewModel" /> that controls the affiliated
        ///     <see cref="InteractionFilterGraph" /> instances
        /// </summary>
        public InteractionFilterGridControlViewModel InteractionFilterGridViewModel { get; }

        /// <summary>
        ///     Get the <see cref="StableEnvironmentGraph" /> of the currently < set <see cref="MocassinProjectGraph" />
        /// </summary>
        public StableEnvironmentGraph Environment => ContentSource?.ProjectModelGraph?.EnergyModelGraph?.StableEnvironment;

        /// <summary>
        ///     Get or set the interaction radius of the currently set <see cref="StableEnvironmentGraph" /> in [Ang]
        /// </summary>
        public double InteractionRadius
        {
            get => Environment?.MaxInteractionRange ?? 0;
            set
            {
                if (Environment == null) return;
                Environment.MaxInteractionRange = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public EnergyParameterControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            InteractionFilterGridViewModel = new InteractionFilterGridControlViewModel(true);
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            InteractionFilterGridViewModel.ChangeContentSource(contentSource);
            InteractionFilterGridViewModel.SetCollection(Environment?.InteractionFilters);
            OnPropertyChanged(nameof(InteractionRadius));
        }
    }
}