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
    ///     manipulation <see cref="ProjectJobTranslationGraph" /> instances
    /// </summary>
    public class JobTranslationControlViewModel : ProjectGraphControlViewModel, IContentSupplier<ProjectJobTranslationGraph>
    {
        /// <summary>
        ///     Get or set the current <see cref="ProjectJobTranslationGraph"/> that serves as a content source
        /// </summary>
        private ProjectJobTranslationGraph JobTranslationContentSource { get; set; }

        /// <summary>
        ///     Get the <see cref="KmcJobPackageControlViewModel"/> that controls the <see cref="KmcJobPackageDescriptionGraph"/> collection
        /// </summary>
        public KmcJobPackageControlViewModel KmcJobPackageViewModel { get; }

        /// <summary>
        ///     Get the <see cref="MmcJobPackageControlViewModel"/> that controls the <see cref="MmcJobPackageDescriptionGraph"/> collection
        /// </summary>
        public MmcJobPackageControlViewModel MmcJobPackageViewModel { get; }

        /// <inheritdoc />
        ProjectJobTranslationGraph IContentSupplier<ProjectJobTranslationGraph>.ContentSource => JobTranslationContentSource;

        /// <inheritdoc />
        public JobTranslationControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            KmcJobPackageViewModel = new KmcJobPackageControlViewModel();
            MmcJobPackageViewModel = new MmcJobPackageControlViewModel();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            KmcJobPackageViewModel.ChangeContentSource(null);
            MmcJobPackageViewModel.ChangeContentSource(null);
        }

        /// <inheritdoc />
        void IContentSupplier<ProjectJobTranslationGraph>.ChangeContentSource(ProjectJobTranslationGraph contentSource)
        {
            KmcJobPackageViewModel.ChangeContentSource(contentSource);
            MmcJobPackageViewModel.ChangeContentSource(contentSource);
            JobTranslationContentSource = contentSource;
        }
    }
}