using System.Collections.Generic;
using System.Linq;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.TransitionModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for the <see cref="MetropolisTransitionGridControlView" /> that
    ///     controls <see cref="MetropolisTransitionData" /> instance definition
    /// </summary>
    public class MetropolisTransitionGridControlViewModel : CollectionControlViewModel<MetropolisTransitionData>,
        IContentSupplier<MocassinProject>
    {
        /// <inheritdoc />
        public MocassinProject ContentSource { get; protected set; }

        /// <summary>
        ///     Get the possible set of <see cref="ModelObjectReference{T}" /> of <see cref="AbstractTransition" /> instances for
        ///     the currently selected
        ///     <see cref="MetropolisTransitionData" />
        /// </summary>
        public IEnumerable<ModelObjectReference<AbstractTransition>> AbstractTransitionOptions => EnumerateAbstractTransitionOptions(SelectedItem);

        /// <summary>
        ///     Get the possible set of <see cref="ModelObjectReference{T}" /> of <see cref="CellReferencePosition" /> instances
        ///     for the currently selected
        ///     <see cref="MetropolisTransitionData" /> for exchanging the first position
        /// </summary>
        public IEnumerable<ModelObjectReference<CellReferencePosition>> FirstPositionOptions => EnumerateReferencePositionOptions(SelectedItem, false);

        /// <summary>
        ///     Get the possible set of <see cref="ModelObjectReference{T}" /> of <see cref="CellReferencePosition" /> instances
        ///     for the currently selected
        ///     <see cref="MetropolisTransitionData" /> for exchanging the second position
        /// </summary>
        public IEnumerable<ModelObjectReference<CellReferencePosition>> SecondPositionOptions => EnumerateReferencePositionOptions(SelectedItem, true);

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelData?.TransitionModelData?.MetropolisTransitions);
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of selectable <see cref="ModelObjectReference{T}" />  to
        ///     <see cref="AbstractTransition" /> instances for the current state of the passed
        ///     <see cref="MetropolisTransitionData" /> in the context of the set <see cref="ContentSource" />
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public IEnumerable<ModelObjectReference<AbstractTransition>> EnumerateAbstractTransitionOptions(MetropolisTransitionData current)
        {
            if (ContentSource == null) yield break;
            if (!(ContentSource.ProjectModelData?.TransitionModelData?.AbstractTransitions is { } source)) yield break;

            var option = new MetropolisTransitionData
            {
                FirstCellReferencePosition = current?.FirstCellReferencePosition,
                SecondCellReferencePosition = current?.SecondCellReferencePosition
            };

            foreach (var abstractTransitionData in source.Where(x => x.StateExchangeGroups.Count == 2))
            {
                option.AbstractTransition = new ModelObjectReference<AbstractTransition>(abstractTransitionData);
                if (current == null || option.Equals(current) || !Items.Where(x => x.Key != current.Key).Contains(option))
                    yield return new ModelObjectReference<AbstractTransition>(abstractTransitionData);
            }
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of selectable <see cref="ModelObjectReference{T}" />  to
        ///     <see cref="CellReferencePosition" /> instances for the current state of the passed
        ///     <see cref="MetropolisTransitionData" /> in the context of the set <see cref="ContentSource" />
        /// </summary>
        /// <param name="current"></param>
        /// <param name="exchangeSecond"></param>
        /// <returns></returns>
        public IEnumerable<ModelObjectReference<CellReferencePosition>> EnumerateReferencePositionOptions(MetropolisTransitionData current, bool exchangeSecond)
        {
            if (ContentSource == null) yield break;
            if (!(ContentSource.ProjectModelData?.StructureModelData?.CellReferencePositions is { } source)) yield break;
            if (current != null && current.AbstractTransition == null) yield break;

            var option = new MetropolisTransitionData
            {
                FirstCellReferencePosition = current?.FirstCellReferencePosition,
                SecondCellReferencePosition = current?.SecondCellReferencePosition,
                AbstractTransition = current?.AbstractTransition
            };

            foreach (var positionData in source.Where(x => x.Stability == PositionStability.Stable))
            {
                if (exchangeSecond)
                    option.SecondCellReferencePosition = new ModelObjectReference<CellReferencePosition>(positionData);
                else
                    option.FirstCellReferencePosition = new ModelObjectReference<CellReferencePosition>(positionData);

                if (current == null || option.Equals(current) || !Items.Where(x => x.Key != current.Key).Contains(option))
                    yield return new ModelObjectReference<CellReferencePosition>(positionData);
            }
        }
    }
}