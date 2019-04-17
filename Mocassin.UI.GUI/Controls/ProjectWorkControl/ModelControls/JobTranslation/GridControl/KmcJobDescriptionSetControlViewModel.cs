using System;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="KmcJobDescriptionSetControlView" /> that controls
    ///     manipulation of <see cref="KmcJobDescriptionGraph" />
    /// </summary>
    public sealed class KmcJobDescriptionSetControlViewModel : CollectionControlViewModel<KmcJobDescriptionGraph>
    {
        /// <summary>
        ///     Gte the <see cref="KmcJobDescriptionGraph" /> that supplies the <see cref="KmcJobDescriptionGraph" /> collection
        /// </summary>
        public KmcJobPackageDescriptionGraph KmcJobPackage { get; }

        /// <summary>
        ///     Creates new <see cref="KmcJobDescriptionSetControlViewModel" /> for the passed
        ///     <see cref="KmcJobPackageDescriptionGraph" />
        /// </summary>
        /// <param name="kmcJobPackage"></param>
        public KmcJobDescriptionSetControlViewModel(KmcJobPackageDescriptionGraph kmcJobPackage)
        {
            KmcJobPackage = kmcJobPackage ?? throw new ArgumentNullException(nameof(kmcJobPackage));
            SetCollection(kmcJobPackage.JobConfigurations);
        }
    }
}