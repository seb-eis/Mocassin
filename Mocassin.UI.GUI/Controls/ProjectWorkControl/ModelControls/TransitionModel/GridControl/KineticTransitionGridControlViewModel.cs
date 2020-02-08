using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Transitions;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for the <see cref="KineticTransitionGridControlView" /> that
    ///     controls <see cref="KineticTransitionData" /> instance definition
    /// </summary>
    public class KineticTransitionGridControlViewModel : CollectionControlViewModel<KineticTransitionData>,
        IContentSupplier<MocassinProject>
    {
        /// <inheritdoc />
        public MocassinProject ContentSource { get; protected set; }

        /// <summary>
        ///     Get the possible set of <see cref="AbstractTransitionData" /> instances for the currently selected
        ///     <see cref="KineticTransitionData" />
        /// </summary>
        public IEnumerable<ModelObjectReference<AbstractTransition>> AbstractTransitionOptions => EnumerateAbstractTransitionOptions(SelectedItem);

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelData?.TransitionModelData?.KineticTransitions);
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of selectable <see cref="ModelObjectReference{T}" /> for
        ///     <see cref="AbstractTransition" /> using the current state of the passed <see cref="KineticTransitionData" /> in the
        ///     context of the set <see cref="ContentSource" />
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public IEnumerable<ModelObjectReference<AbstractTransition>> EnumerateAbstractTransitionOptions(KineticTransitionData current)
        {
            return ContentSource?.ProjectModelData?.TransitionModelData?.AbstractTransitions
                .Where(x => x.StateExchangeGroups.Count > 2)
                .Select(x => new ModelObjectReference<AbstractTransition>(x));
        }
    }
}