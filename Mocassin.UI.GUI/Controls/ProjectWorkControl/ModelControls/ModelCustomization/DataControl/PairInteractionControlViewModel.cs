using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    ///     <see cref="PairEnergySetData" /> customization data
    /// </summary>
    public class PairInteractionControlViewModel : CollectionControlViewModel<PairEnergySetControlViewModel>, IContentSupplier<ProjectCustomizationTemplate>,
        IDisposable
    {
        /// <summary>
        ///     Get the <see cref="Func{T,TResult}" /> getter that provides the <see cref="IReadOnlyList{T}" /> of
        ///     <see cref="PairEnergySetData" />
        /// </summary>
        private Func<ProjectCustomizationTemplate, IReadOnlyList<PairEnergySetData>> InteractionSetGetter { get; }

        /// <inheritdoc />
        public ProjectCustomizationTemplate ContentSource { get; protected set; }

        /// <summary>
        ///     Creates new <see cref="PairInteractionControlViewModel" /> with the provided getter <see cref="Func{T,TResult}" />
        ///     for the target collection
        /// </summary>
        /// <param name="interactionSetGetter"></param>
        public PairInteractionControlViewModel(Func<ProjectCustomizationTemplate, IReadOnlyList<PairEnergySetData>> interactionSetGetter)
        {
            InteractionSetGetter = interactionSetGetter ?? throw new ArgumentNullException(nameof(interactionSetGetter));
        }

        /// <inheritdoc />
        public void ChangeContentSource(ProjectCustomizationTemplate contentSource)
        {
            ContentSource = contentSource;
            UpdateSetControlViewModels();
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            DisposeAndClearSetControlViewModels();
        }

        /// <summary>
        ///     Updates the required <see cref="PairEnergySetControlViewModel" /> collection
        /// </summary>
        private void UpdateSetControlViewModels()
        {
            Items ??= new ObservableCollection<PairEnergySetControlViewModel>();

            DisposeAndClearSetControlViewModels();
            var interactionSets = InteractionSetGetter.Invoke(ContentSource);
            if (interactionSets == null) return;

            Items.AddRange(interactionSets.Select(pairEnergySetData => new PairEnergySetControlViewModel(pairEnergySetData, interactionSets)));
        }

        /// <summary>
        ///     Disposes the current set control view models and clears the collection
        /// </summary>
        private void DisposeAndClearSetControlViewModels()
        {
            foreach (var viewModel in Items ?? Enumerable.Empty<PairEnergySetControlViewModel>()) viewModel?.Dispose();
            Items?.Clear();
        }
    }
}