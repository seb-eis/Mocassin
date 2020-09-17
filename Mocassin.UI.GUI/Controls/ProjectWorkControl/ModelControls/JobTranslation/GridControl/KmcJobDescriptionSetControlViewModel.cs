using System;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="KmcJobDescriptionSetControlView" /> that controls
    ///     manipulation of <see cref="KmcJobConfigData" />
    /// </summary>
    public sealed class KmcJobDescriptionSetControlViewModel : CollectionControlViewModel<KmcJobConfigData>
    {
        private int duplicateCount = 1;

        /// <summary>
        ///     Get the <see cref="KmcJobConfigData" /> that supplies the <see cref="KmcJobConfigData" /> collection
        /// </summary>
        public KmcJobPackageData KmcJobPackage { get; }

        /// <summary>
        ///     Get the parent <see cref="MocassinProject" />
        /// </summary>
        public MocassinProject Project { get; }

        /// <summary>
        ///     Get the <see cref="DuplicateCollectionItemCommand{T}" /> for the collection
        /// </summary>
        public DuplicateCollectionItemCommand<KmcJobConfigData> DuplicateItemCommand { get; }

        /// <summary>
        ///     Get or set the duplicate count if the duplicate command is executed
        /// </summary>
        public int DuplicateCount
        {
            get => duplicateCount;
            set => SetProperty(ref duplicateCount, value > 0 ? value : 1);
        }

        /// <summary>
        ///     Creates new <see cref="KmcJobDescriptionSetControlViewModel" /> for the passed
        ///     <see cref="KmcJobPackageData" />
        /// </summary>
        /// <param name="kmcJobPackage"></param>
        /// <param name="project"></param>
        public KmcJobDescriptionSetControlViewModel(KmcJobPackageData kmcJobPackage, MocassinProject project)
        {
            KmcJobPackage = kmcJobPackage ?? throw new ArgumentNullException(nameof(kmcJobPackage));
            Project = project ?? throw new ArgumentNullException(nameof(project));
            SetCollection(kmcJobPackage.JobConfigurations);
            DuplicateItemCommand = new DuplicateCollectionItemCommand<KmcJobConfigData>(this) {CountProvider = () => DuplicateCount};
        }
    }
}