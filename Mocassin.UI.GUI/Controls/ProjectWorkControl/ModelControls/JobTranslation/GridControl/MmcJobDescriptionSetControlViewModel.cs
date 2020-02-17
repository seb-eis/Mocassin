using System;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="MmcJobDescriptionSetControlView" /> that controls
    ///     manipulation of <see cref="MmcJobConfigData" />
    /// </summary>
    public sealed class MmcJobDescriptionSetControlViewModel : CollectionControlViewModel<MmcJobConfigData>
    {
        private int duplicateCount = 1;

        /// <summary>
        ///     Get the <see cref="MmcJobConfigData" /> that supplies the <see cref="MmcJobConfigData" /> collection
        /// </summary>
        public MmcJobPackageData MmcJobPackage { get; }

        /// <summary>
        ///     The parent <see cref="MocassinProject" />
        /// </summary>
        public MocassinProject Project { get; }

        /// <summary>
        ///     Get the <see cref="DuplicateCollectionItemCommand{T}" /> for the collection
        /// </summary>
        public DuplicateCollectionItemCommand<MmcJobConfigData> DuplicateItemCommand { get; }

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
        ///     <see cref="MmcJobPackageData" />
        /// </summary>
        /// <param name="mmcJobPackage"></param>
        /// <param name="project"></param>
        public MmcJobDescriptionSetControlViewModel(MmcJobPackageData mmcJobPackage, MocassinProject project)
        {
            MmcJobPackage = mmcJobPackage ?? throw new ArgumentNullException(nameof(mmcJobPackage));
            Project = project ?? throw new ArgumentNullException(nameof(project));
            SetCollection(mmcJobPackage.JobConfigurations);
            DuplicateItemCommand = new DuplicateCollectionItemCommand<MmcJobConfigData>(this) {CountProvider = () => DuplicateCount};
        }
    }
}