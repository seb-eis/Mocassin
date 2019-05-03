using System;
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
        /// <summary>
        ///     Get the <see cref="MmcJobDescriptionGraph" /> that supplies the <see cref="MmcJobDescriptionGraph" /> collection
        /// </summary>
        public MmcJobPackageDescriptionGraph MmcJobPackage { get; }

        public MocassinProjectGraph ProjectGraph { get; }

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
        }
    }
}