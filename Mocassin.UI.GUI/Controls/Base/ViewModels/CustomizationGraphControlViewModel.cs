using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.Base.ViewModels
{
    /// <summary>
    ///     Base class for aa <see cref="PrimaryControlViewModel" /> types that control
    ///     <see cref="ProjectCustomizationTemplate" /> content
    /// </summary>
    public abstract class CustomizationGraphControlViewModel : PrimaryControlViewModel, IContentSupplier<ProjectCustomizationTemplate>
    {
        /// <inheritdoc />
        public ProjectCustomizationTemplate ContentSource { get; protected set; }

        /// <inheritdoc />
        protected CustomizationGraphControlViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public abstract void ChangeContentSource(ProjectCustomizationTemplate contentSource);
    }
}