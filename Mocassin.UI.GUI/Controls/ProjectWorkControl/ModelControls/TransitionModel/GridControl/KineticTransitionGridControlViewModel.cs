using System;
using System.Collections.Generic;
using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Symmetry.SpaceGroups;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for the <see cref="KineticTransitionGridControlView" /> that
    ///     controls <see cref="KineticTransitionGraph" /> instance definition
    /// </summary>
    public class KineticTransitionGridControlViewModel : CollectionControlViewModel<KineticTransitionGraph>,
        IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Get the possible set of <see cref="AbstractTransitionGraph" /> instances for the currently selected
        ///     <see cref="KineticTransitionGraph" />
        /// </summary>
        public IEnumerable<AbstractTransitionGraph> AbstractTransitionOptions => GetAbstractOptions(SelectedCollectionItem);

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelGraph?.TransitionModelGraph?.KineticTransitions);
        }
        
        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of selectable <see cref="AbstractTransitionGraph" /> for the current state of
        ///     the passed <see cref="KineticTransitionGraph" /> in the context of the set <see cref="ContentSource" />
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public IEnumerable<AbstractTransitionGraph> GetAbstractOptions(KineticTransitionGraph current)
        {
            return ContentSource?.ProjectModelGraph?.TransitionModelGraph
                ?.AbstractTransitions.Where(x => x.StateExchangeGroups.Count > 2);
        }
    }
}