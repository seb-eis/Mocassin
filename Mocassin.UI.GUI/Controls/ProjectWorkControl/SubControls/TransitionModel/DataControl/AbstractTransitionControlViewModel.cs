﻿using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.TransitionModel.GridControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.TransitionModel.DataControl
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="AbstractTransitionControlView" /> that controls
    ///     transition abstraction data
    /// </summary>
    public class AbstractTransitionControlViewModel : PrimaryControlViewModel, IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Get the <see cref="ExchangePairGridControlViewModel"/> that controls the collection of <see cref="StateExchangePairGraph"/>
        /// </summary>
        public ExchangePairGridControlViewModel ExchangePairControlViewModel { get; }

        /// <inheritdoc />
        public AbstractTransitionControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ExchangePairControlViewModel = new ExchangePairGridControlViewModel();
        }

        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            if (contentSource is MocassinProjectGraph projectGraph) ChangeContentSource(projectGraph);
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            ExchangePairControlViewModel.ChangeContentSource(contentSource);
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ChangeContentSource(null);
        }
    }
}