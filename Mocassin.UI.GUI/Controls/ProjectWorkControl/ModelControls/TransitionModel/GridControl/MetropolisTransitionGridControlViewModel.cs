using System.Collections.Generic;
using System.Linq;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.StructureModel;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for the <see cref="MetropolisTransitionGridControlView" /> that
    ///     controls <see cref="MetropolisTransitionGraph" /> instance definition
    /// </summary>
    public class MetropolisTransitionGridControlViewModel : CollectionControlViewModel<MetropolisTransitionGraph>,
        IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Get the possible set of <see cref="AbstractTransitionGraph" /> instances for the currently selected
        ///     <see cref="MetropolisTransitionGraph" />
        /// </summary>
        public IEnumerable<AbstractTransitionGraph> AbstractTransitionOptions => GetAbstractOptions(SelectedItem);

        /// <summary>
        ///     Get the possible set of <see cref="UnitCellPositionGraph" /> instances for the currently selected
        ///     <see cref="MetropolisTransitionGraph" /> for exchanging the first key
        /// </summary>
        public IEnumerable<UnitCellPositionGraph> FirstWyckoffOptions => GetWyckoffOptions(SelectedItem, false);

        /// <summary>
        ///     Get the possible set of <see cref="UnitCellPositionGraph" /> instances for the currently selected
        ///     <see cref="MetropolisTransitionGraph" /> for exchanging the second key
        /// </summary>
        public IEnumerable<UnitCellPositionGraph> SecondWyckoffOptions => GetWyckoffOptions(SelectedItem, true);

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelGraph?.TransitionModelGraph?.MetropolisTransitions);
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of selectable <see cref="AbstractTransitionGraph" /> for the current state of
        ///     the passed <see cref="MetropolisTransitionGraph" /> in the context of the set <see cref="ContentSource" />
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public IEnumerable<AbstractTransitionGraph> GetAbstractOptions(MetropolisTransitionGraph current)
        {
            if (ContentSource == null) yield break;
            if (!(ContentSource.ProjectModelGraph?.TransitionModelGraph?.AbstractTransitions is var source)) yield break;

            var option = new MetropolisTransitionGraph
            {
                FirstUnitCellPositionKey = current?.FirstUnitCellPositionKey,
                SecondUnitCellPositionKey = current?.SecondUnitCellPositionKey
            };

            foreach (var transitionGraph in source.Where(x => x.StateExchangeGroups.Count == 2))
            {
                option.AbstractTransitionKey = transitionGraph.Key;
                if (current == null || option.Equals(current) || !Items.Where(x => x.Key != current.Key).Contains(option))
                    yield return transitionGraph;
            }
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of selectable <see cref="UnitCellPositionGraph" /> for the current state of
        ///     the passed <see cref="MetropolisTransitionGraph" /> in the context of the set <see cref="ContentSource" />
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public IEnumerable<UnitCellPositionGraph> GetWyckoffOptions(MetropolisTransitionGraph current, bool exchangeSecond)
        {
            if (ContentSource == null) yield break;
            if (!(ContentSource.ProjectModelGraph?.StructureModelGraph?.UnitCellPositions is var source)) yield break;
            if (current != null && current.AbstractTransitionKey == null) yield break;

            var option = new MetropolisTransitionGraph
            {
                FirstUnitCellPositionKey = current?.FirstUnitCellPositionKey,
                SecondUnitCellPositionKey = current?.SecondUnitCellPositionKey,
                AbstractTransitionKey = current?.AbstractTransitionKey
            };

            foreach (var positionGraph in source)
            {
                if (exchangeSecond)
                    option.SecondUnitCellPositionKey = positionGraph.Key;
                else
                    option.FirstUnitCellPositionKey = positionGraph.Key;

                if (current == null || option.Equals(current) || !Items.Where(x => x.Key != current.Key).Contains(option))
                    yield return positionGraph;
            }
        }
    }
}