using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Mocassin.Model.Structures;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.EnergyModel;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> implementation for
    ///     <see cref="UnstableEnvironmentGridControlView" /> that controls <see cref="UnstableEnvironmentGraph" /> definitions
    /// </summary>
    public class UnstableEnvironmentGridControlViewModel : CollectionControlViewModel<UnstableEnvironmentGraph>,
        IContentSupplier<MocassinProjectGraph>
    {
        private MocassinProjectGraph contentSource;

        /// <inheritdoc />
        public MocassinProjectGraph ContentSource
        {
            get => contentSource;
            protected set => SetProperty(ref contentSource, value);
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> sequence of all available unstable <see cref="UnitCellPositionGraph" />
        ///     instances
        /// </summary>
        public IEnumerable<UnitCellPositionGraph> UnstablePositionOptions => GetUnstablePositions();

        /// <summary>
        ///     Get a <see cref="ICommand"/> to resynchronize the environment collection
        /// </summary>
        public ICommand SynchronizeEnvironmentCollectionCommand { get; }

        /// <summary>
        ///     Creates new <see cref="UnstableEnvironmentGridControlViewModel"/>
        /// </summary>
        public UnstableEnvironmentGridControlViewModel()
        {
            SynchronizeEnvironmentCollectionCommand = new RelayCommand(() => EnsureEnvironmentToPositionSync(DataCollection));
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            SetCollection(ContentSource?.ProjectModelGraph?.EnergyModelGraph?.UnstableEnvironments);
        }

        /// <inheritdoc />
        public override void SetCollection(ICollection<UnstableEnvironmentGraph> collection)
        {
            EnsureEnvironmentToPositionSync(collection);
            base.SetCollection(collection);
        }

        /// <summary>
        ///     Ensures that the set of <see cref="UnstableEnvironmentGraph" /> contains only existing entries of positions and
        ///     that all unstable entries are created
        /// </summary>
        /// <param name="collection"></param>
        public void EnsureEnvironmentToPositionSync(ICollection<UnstableEnvironmentGraph> collection)
        {
            if (collection == null) return;

            var positions = GetUnstablePositions()?.ToList();
            if (positions == null) return;

            if (positions.Count == collection.Count && collection.All(x => positions.Any(y => x.UnitCellPositionKey == y.Key)))
                return;

            var originalEnvironments = collection.ToList();
            collection.Clear();

            foreach (var position in positions)
            {
                if (originalEnvironments.FirstOrDefault(x => x.UnitCellPositionKey == position.Key) is UnstableEnvironmentGraph environment)
                    collection.Add(environment);
                else
                    collection.Add(new UnstableEnvironmentGraph {UnitCellPositionKey = position.Key});
            }
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of all <see cref="UnitCellPositionGraph" /> instances that describes unstable
        ///     positions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UnitCellPositionGraph> GetUnstablePositions()
        {
            return ContentSource?.ProjectModelGraph?.StructureModelGraph?.UnitCellPositions
                ?.Where(x => x.PositionStatus == PositionStatus.Unstable);
        }
    }
}