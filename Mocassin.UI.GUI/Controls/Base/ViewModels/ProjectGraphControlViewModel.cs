using System;
using System.Threading;
using System.Threading.Tasks;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.Base.ViewModels
{
    /// <summary>
    ///     Base class for all <see cref="PrimaryControlViewModel" /> instances that control
    ///     <see cref="MocassinProjectGraph" /> content
    /// </summary>
    public abstract class ProjectGraphControlViewModel : PrimaryControlViewModel, IContentSupplier<MocassinProjectGraph>
    {
        private MocassinProjectGraph contentSource;

        /// <inheritdoc />
        public MocassinProjectGraph ContentSource
        {
            get => contentSource;
            protected set => SetProperty(ref contentSource, value);
        }

        /// <inheritdoc />
        protected ProjectGraphControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        public abstract void ChangeContentSource(MocassinProjectGraph contentSource);

        /// <summary>
        ///     Delayed execution of the passed <see cref="Action"/>. Action is not performed if the <see cref="ContentSource"/> property changes within the delay
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        /// <param name="onAppThread"></param>
        protected Task ExecuteIfContentSourceUnchanged(Action action, TimeSpan delay, bool onAppThread = false)
        {
            return ExecuteIfPropertyUnchanged(action, delay, nameof(ContentSource), onAppThread);
        }
    }
}