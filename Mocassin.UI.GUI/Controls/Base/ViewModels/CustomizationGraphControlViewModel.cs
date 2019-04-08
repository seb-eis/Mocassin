using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.Base.ViewModels
{
    /// <summary>
    ///     Base class for aa <see cref="PrimaryControlViewModel" /> types that control
    ///     <see cref="ProjectCustomizationGraph" /> content
    /// </summary>
    public abstract class CustomizationGraphControlViewModel : PrimaryControlViewModel, IContentSupplier<ProjectCustomizationGraph>
    {
        /// <inheritdoc />
        public ProjectCustomizationGraph ContentSource { get; protected set; }

        /// <inheritdoc />
        protected CustomizationGraphControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            if (contentSource is ProjectCustomizationGraph customizationGraph) ChangeContentSource(customizationGraph);
        }

        /// <inheritdoc />
        public abstract void ChangeContentSource(ProjectCustomizationGraph contentSource);
    }
}