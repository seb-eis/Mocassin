using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Mocassin.Model.Structures;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.EnergyModel;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> implementation for
    ///     <see cref="UnstableEnvironmentGridControlView" /> that controls <see cref="UnstableEnvironmentData" /> definitions
    /// </summary>
    public class UnstableEnvironmentGridControlViewModel : CollectionControlViewModel<UnstableEnvironmentData>,
        IContentSupplier<MocassinProject>
    {
        private MocassinProject contentSource;

        /// <inheritdoc />
        public MocassinProject ContentSource
        {
            get => contentSource;
            protected set => SetProperty(ref contentSource, value);
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> sequence of <see cref="ModelObjectReference{T}" /> for unstable
        ///     <see cref="CellSite" /> instances
        /// </summary>
        public IEnumerable<ModelObjectReference<CellSite>> UnstablePositionOptions => EnumerateUnstableReferencePositions();

        /// <summary>
        ///     Get a <see cref="ICommand" /> to resynchronize the environment collection
        /// </summary>
        public ICommand SynchronizeEnvironmentCollectionCommand { get; }

        /// <summary>
        ///     Creates new <see cref="UnstableEnvironmentGridControlViewModel" />
        /// </summary>
        public UnstableEnvironmentGridControlViewModel()
        {
            SynchronizeEnvironmentCollectionCommand = new RelayCommand(() => EnsureEnvironmentToPositionSync(Items));
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProject project)
        {
            ContentSource = project;
            SetCollection(ContentSource?.ProjectModelData?.EnergyModelData?.UnstableEnvironments);
        }

        /// <inheritdoc />
        public override void SetCollection(ICollection<UnstableEnvironmentData> collection)
        {
            EnsureEnvironmentToPositionSync(collection);
            base.SetCollection(collection);
        }

        /// <summary>
        ///     Ensures that the set of <see cref="UnstableEnvironmentData" /> contains only existing entries of positions and
        ///     that all unstable entries are created
        /// </summary>
        /// <param name="collection"></param>
        public void EnsureEnvironmentToPositionSync(ICollection<UnstableEnvironmentData> collection)
        {
            if (collection == null) return;

            var positionReferences = EnumerateUnstableReferencePositions()?.ToList();
            if (positionReferences == null) return;

            if (positionReferences.Count == collection.Count && collection.All(x => positionReferences.Any(y => x.CellReferencePosition.Key == y.Key)))
                return;

            var originalEnvironments = collection.ToList();
            collection.Clear();

            foreach (var positionReference in positionReferences)
            {
                collection.Add(originalEnvironments.FirstOrDefault(x => x.CellReferencePosition?.Key == positionReference.Key) is { } environment
                    ? environment
                    : new UnstableEnvironmentData {CellReferencePosition = positionReference.Duplicate()});
            }
        }

        /// <summary>
        ///     Get an <see cref="IEnumerable{T}" /> of <see cref="ModelObjectReference{T}" /> for
        ///     <see cref="CellSite" /> instances that describes unstable positions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModelObjectReference<CellSite>> EnumerateUnstableReferencePositions()
        {
            return ContentSource?.ProjectModelData?.StructureModelData?.CellReferencePositions
                                ?.Where(x => x.Stability == PositionStability.Unstable)
                                .Select(x => new ModelObjectReference<CellSite>(x));
        }
    }
}