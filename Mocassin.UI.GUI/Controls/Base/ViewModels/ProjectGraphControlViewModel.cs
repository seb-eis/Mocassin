using System;
using System.Threading.Tasks;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.Base.ViewModels
{
    /// <summary>
    ///     Base class for all <see cref="PrimaryControlViewModel" /> instances that control
    ///     <see cref="MocassinProject" /> content
    /// </summary>
    public abstract class ProjectGraphControlViewModel : PrimaryControlViewModel, IContentSupplier<MocassinProject>
    {
        private MocassinProject contentSource;

        /// <inheritdoc />
        public MocassinProject ContentSource
        {
            get => contentSource;
            protected set => SetProperty(ref contentSource, value);
        }

        /// <inheritdoc />
        protected ProjectGraphControlViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public abstract void ChangeContentSource(MocassinProject contentSource);

        /// <summary>
        ///     Delayed execution of the passed <see cref="Action" />. Action is not performed if the <see cref="ContentSource" />
        ///     property changes within the delay
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        /// <param name="onAppThread"></param>
        protected Task ExecuteIfContentSourceUnchanged(Action action, TimeSpan delay, bool onAppThread = false) =>
            ExecuteIfPropertyUnchanged(action, delay, nameof(ContentSource), onAppThread);
    }
}