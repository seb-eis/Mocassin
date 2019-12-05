using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Extensions;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.GridControl;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.DataControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for the <see cref="PairInteractionControlView" /> that control
    ///     <see cref="PairEnergySetGraph" /> customization data
    /// </summary>
    public class PairInteractionControlViewModel : CollectionControlViewModel<PairEnergySetControlViewModel>, IContentSupplier<ProjectCustomizationGraph>, IDisposable
    {
        /// <summary>
        ///     Get the <see cref="Func{T,TResult}" /> getter that provides the <see cref="IReadOnlyList{T}" /> of
        ///     <see cref="PairEnergySetGraph" />
        /// </summary>
        private Func<ProjectCustomizationGraph, IReadOnlyList<PairEnergySetGraph>> InteractionSetGetter { get; }

        /// <inheritdoc />
        public ProjectCustomizationGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Creates new <see cref="PairInteractionControlViewModel" /> with the provided getter <see cref="Func{T,TResult}" />
        ///     for the target collection
        /// </summary>
        /// <param name="interactionSetGetter"></param>
        public PairInteractionControlViewModel(Func<ProjectCustomizationGraph, IReadOnlyList<PairEnergySetGraph>> interactionSetGetter)
        {
            InteractionSetGetter = interactionSetGetter ?? throw new ArgumentNullException(nameof(interactionSetGetter));
        }

        /// <inheritdoc />
        public void ChangeContentSource(ProjectCustomizationGraph contentSource)
        {
            ContentSource = contentSource;
            CreateSetControlViewModels();
        }

        /// <summary>
        ///     Creates and sets the new <see cref="PairEnergySetControlViewModel" /> collection
        /// </summary>
        private void CreateSetControlViewModels()
        {
            var interactionSets = InteractionSetGetter.Invoke(ContentSource);
            if (interactionSets == null)
            {
                SetCollection(null);
                return;
            }
            var viewModels = interactionSets.Select(x => new PairEnergySetControlViewModel(x, interactionSets)).ToList(interactionSets.Count);
            SetCollection(viewModels);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var item in Items ?? Enumerable.Empty<PairEnergySetControlViewModel>()) item?.Dispose();
        }
    }
}