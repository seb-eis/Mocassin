using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.GridControl;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.EnergyModel;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="RadialInteractionFilterData" /> and radius
    ///     definitions
    ///     of the stable environment
    /// </summary>
    public class EnergyParameterControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="InteractionFilterGridControlViewModel" /> that controls the affiliated
        ///     <see cref="RadialInteractionFilterData" /> instances
        /// </summary>
        public InteractionFilterGridControlViewModel InteractionFilterGridViewModel { get; }

        /// <summary>
        ///     Get the <see cref="StableEnvironmentData" /> of the currently set <see cref="MocassinProject" />
        /// </summary>
        public StableEnvironmentData Environment => ContentSource?.ProjectModelData?.EnergyModelData?.StableEnvironment;

        /// <summary>
        ///     Get or set the interaction radius of the currently set <see cref="StableEnvironmentData" /> in [Ang]
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
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            InteractionFilterGridViewModel.ChangeContentSource(contentSource);
            InteractionFilterGridViewModel.SetCollection(Environment?.InteractionFilters);
            OnPropertyChanged(nameof(InteractionRadius));
        }
    }
}