using System;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="KmcJobDescriptionSetControlView" /> that controls
    ///     manipulation of <see cref="KmcJobDescriptionGraph" />
    /// </summary>
    public sealed class KmcJobDescriptionSetControlViewModel : CollectionControlViewModel<KmcJobDescriptionGraph>
    {
        /// <summary>
        ///     Get the <see cref="KmcJobDescriptionGraph" /> that supplies the <see cref="KmcJobDescriptionGraph" /> collection
        /// </summary>
        public KmcJobPackageDescriptionGraph KmcJobPackage { get; }

        /// <summary>
        ///     Get the parent <see cref="MocassinProjectGraph" />
        /// </summary>
        public MocassinProjectGraph ProjectGraph { get; }

        /// <summary>
        ///     Creates new <see cref="KmcJobDescriptionSetControlViewModel" /> for the passed
        ///     <see cref="KmcJobPackageDescriptionGraph" />
        /// </summary>
        /// <param name="kmcJobPackage"></param>
        /// <param name="project"></param>
        public KmcJobDescriptionSetControlViewModel(KmcJobPackageDescriptionGraph kmcJobPackage, MocassinProjectGraph project)
        {
            KmcJobPackage = kmcJobPackage ?? throw new ArgumentNullException(nameof(kmcJobPackage));
            ProjectGraph = project ?? throw new ArgumentNullException(nameof(project));
            SetCollection(kmcJobPackage.JobConfigurations);
        }
    }
}