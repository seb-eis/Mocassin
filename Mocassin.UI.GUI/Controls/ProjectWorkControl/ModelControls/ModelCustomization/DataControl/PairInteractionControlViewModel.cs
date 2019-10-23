using System;
using System.Collections.Generic;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for the <see cref="PairInteractionControlView" /> that control
    ///     <see cref="PairInteractionGraph" /> customization data
    /// </summary>
    public class PairInteractionControlViewModel : CollectionControlViewModel<PairInteractionGraph>, IContentSupplier<ProjectCustomizationGraph>
    {
        /// <summary>
        /// Get the <see cref="Func{T,TResult}"/> getter that provides the <see cref="ICollection{T}"/> of <see cref="PairInteractionGraph"/>
        /// </summary>
        private Func<ProjectCustomizationGraph, ICollection<PairInteractionGraph>> InteractionSetGetter { get; }

        /// <inheritdoc />
        public ProjectCustomizationGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Creates new <see cref="PairInteractionControlViewModel"/> with the provided getter <see cref="Func{T,TResult}"/> for the target collection
        /// </summary>
        /// <param name="interactionSetGetter"></param>
        public PairInteractionControlViewModel(Func<ProjectCustomizationGraph, ICollection<PairInteractionGraph>> interactionSetGetter)
        {
            InteractionSetGetter = interactionSetGetter ?? throw new ArgumentNullException(nameof(interactionSetGetter));
        }

        /// <inheritdoc />
        public void ChangeContentSource(ProjectCustomizationGraph contentSource)
        {
            ContentSource = contentSource;
            SetCollection(InteractionSetGetter(contentSource));
        }
    }
}