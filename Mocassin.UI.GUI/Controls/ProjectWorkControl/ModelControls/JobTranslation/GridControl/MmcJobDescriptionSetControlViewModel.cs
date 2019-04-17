using System;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Jobs;

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

        /// <summary>
        ///     Creates new <see cref="MmcJobDescriptionSetControlViewModel" /> for the passed
        ///     <see cref="MmcJobPackageDescriptionGraph" />
        /// </summary>
        /// <param name="mmcJobPackage"></param>
        public MmcJobDescriptionSetControlViewModel(MmcJobPackageDescriptionGraph mmcJobPackage)
        {
            MmcJobPackage = mmcJobPackage ?? throw new ArgumentNullException(nameof(mmcJobPackage));
            SetCollection(mmcJobPackage.JobConfigurations);
        }
    }
}