using System;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.DataControl;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="JobTranslationControlView" /> that supplies content for
    ///     manipulation <see cref="ProjectJobSetTemplate" /> instances
    /// </summary>
    public class JobTranslationControlViewModel : ProjectGraphControlViewModel, IContentSupplier<ProjectJobSetTemplate>
    {
        /// <summary>
        ///     Get or set the current <see cref="ProjectJobSetTemplate"/> that serves as a content source
        /// </summary>
        private ProjectJobSetTemplate JobTranslationContentSource { get; set; }

        /// <summary>
        ///     Get the <see cref="KmcJobPackageControlViewModel"/> that controls the <see cref="KmcJobPackageData"/> collection
        /// </summary>
        public KmcJobPackageControlViewModel KmcJobPackageViewModel { get; }

        /// <summary>
        ///     Get the <see cref="MmcJobPackageControlViewModel"/> that controls the <see cref="MmcJobPackageData"/> collection
        /// </summary>
        public MmcJobPackageControlViewModel MmcJobPackageViewModel { get; }

        /// <inheritdoc />
        ProjectJobSetTemplate IContentSupplier<ProjectJobSetTemplate>.ContentSource => JobTranslationContentSource;

        /// <inheritdoc />
        public JobTranslationControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            KmcJobPackageViewModel = new KmcJobPackageControlViewModel();
            MmcJobPackageViewModel = new MmcJobPackageControlViewModel();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            KmcJobPackageViewModel.ChangeContentSource(null);
            MmcJobPackageViewModel.ChangeContentSource(null);
        }

        /// <inheritdoc />
        void IContentSupplier<ProjectJobSetTemplate>.ChangeContentSource(ProjectJobSetTemplate contentSource)
        {
            KmcJobPackageViewModel.ChangeContentSource(contentSource);
            MmcJobPackageViewModel.ChangeContentSource(contentSource);
            JobTranslationContentSource = contentSource;
        }
    }
}