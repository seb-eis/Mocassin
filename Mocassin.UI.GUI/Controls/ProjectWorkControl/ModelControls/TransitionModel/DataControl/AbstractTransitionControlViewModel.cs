using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="AbstractTransitionControlView" /> that controls
    ///     transition abstraction data
    /// </summary>
    public class AbstractTransitionControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="ExchangePairGridControlViewModel" /> that controls the collection of
        ///     <see cref="StateExchangePairData" /> instances
        /// </summary>
        public ExchangePairGridControlViewModel ExchangePairGridViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ExchangeGroupGridControlViewModel" /> that controls the collection of
        ///     <see cref="StateExchangeGroupData" /> instances
        /// </summary>
        public ExchangeGroupGridControlViewModel ExchangeGroupGridViewModel { get; }

        /// <summary>
        ///     Get the <see cref="AbstractTransitionGridControlViewModel" /> that controls the collection of
        ///     <see cref="AbstractTransitionData" /> instances
        /// </summary>
        public AbstractTransitionGridControlViewModel AbstractTransitionGridViewModel { get; }

        /// <inheritdoc />
        public AbstractTransitionControlViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            ExchangePairGridViewModel = new ExchangePairGridControlViewModel();
            ExchangeGroupGridViewModel = new ExchangeGroupGridControlViewModel();
            AbstractTransitionGridViewModel = new AbstractTransitionGridControlViewModel();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            ExchangePairGridViewModel.ChangeContentSource(contentSource);
            ExchangeGroupGridViewModel.ChangeContentSource(contentSource);
            AbstractTransitionGridViewModel.ChangeContentSource(contentSource);
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ChangeContentSource(null);
        }
    }
}