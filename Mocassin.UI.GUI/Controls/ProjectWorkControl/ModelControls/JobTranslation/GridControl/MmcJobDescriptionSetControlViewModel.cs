using System;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="MmcJobDescriptionSetControlView" /> that controls
    ///     manipulation of <see cref="MmcJobDescriptionGraph" />
    /// </summary>
    public sealed class MmcJobDescriptionSetControlViewModel : CollectionControlViewModel<MmcJobDescriptionGraph>
    {
        private int duplicateCount = 1;

        /// <summary>
        ///     Get the <see cref="MmcJobDescriptionGraph" /> that supplies the <see cref="MmcJobDescriptionGraph" /> collection
        /// </summary>
        public MmcJobPackageDescriptionGraph MmcJobPackage { get; }

        /// <summary>
        ///     The parent <see cref="MocassinProjectGraph"/>
        /// </summary>
        public MocassinProjectGraph ProjectGraph { get; }

        /// <summary>
        ///     Get the <see cref="DuplicateCollectionItemCommand{T}"/> for the collection
        /// </summary>
        public DuplicateCollectionItemCommand<MmcJobDescriptionGraph> DuplicateItemCommand { get; }

        /// <summary>
        ///     Get or set the duplicate count if the duplicate command is executed
        /// </summary>
        public int DuplicateCount
        {
            get => duplicateCount;
            set => SetProperty(ref duplicateCount, value > 0 ? value : 1);
        }

        /// <summary>
        ///     Creates new <see cref="MmcJobDescriptionSetControlViewModel" /> for the passed
        ///     <see cref="MmcJobPackageDescriptionGraph" />
        /// </summary>
        /// <param name="mmcJobPackage"></param>
        /// <param name="projectGraph"></param>
        public MmcJobDescriptionSetControlViewModel(MmcJobPackageDescriptionGraph mmcJobPackage, MocassinProjectGraph projectGraph)
        {
            MmcJobPackage = mmcJobPackage ?? throw new ArgumentNullException(nameof(mmcJobPackage));
            ProjectGraph = projectGraph ?? throw new ArgumentNullException(nameof(projectGraph));
            SetCollection(mmcJobPackage.JobConfigurations);
            DuplicateItemCommand = new DuplicateCollectionItemCommand<MmcJobDescriptionGraph>(this) {CountProvider = () => DuplicateCount};
        }
    }
}