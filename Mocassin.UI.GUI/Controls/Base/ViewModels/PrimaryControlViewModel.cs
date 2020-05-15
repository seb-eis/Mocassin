using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Runtime.CompilerServices;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Messaging;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.Base.ViewModels
{
    /// <summary>
    ///     Base <see cref="ViewModelBase" /> implementation for primary controls with access to the components of the
    ///     <see cref="IProjectAppControl" />
    /// </summary>
    public abstract class PrimaryControlViewModel : ViewModelBase, IDisposable
    {
        /// <summary>
        ///     Get the <see cref="IDisposable" /> subscription to the <see cref="IMocassinProjectLibrary" /> replacements
        /// </summary>
        private IDisposable ProjectLibraryChangeSubscription { get; }

        /// <summary>
        ///     Get the <see cref="IDisposable" /> subscription to the <see cref="IMocassinProjectLibrary" /> entity changes
        /// </summary>
        private IDisposable ProjectEntityChangeSubscription { get; set; }

        /// <summary>
        ///     Get the main <see cref="IProjectAppControl" /> that this sub control is connected to
        /// </summary>
        public IProjectAppControl ProjectControl { get; }

        /// <summary>
        ///     Create new <see cref="PrimaryControlViewModel" /> that is connected to the passed
        ///     <see cref="IProjectAppControl" />
        /// </summary>
        /// <param name="projectControl"></param>
        protected PrimaryControlViewModel(IProjectAppControl projectControl)
        {
            ProjectControl = projectControl ?? throw new ArgumentNullException(nameof(projectControl));
            ProjectLibraryChangeSubscription = projectControl.ProjectLibraryChangeNotification
                .Subscribe(OnProjectLibraryChanged);

            SwitchLibraryEntityChangeSubscription(ProjectControl.OpenProjectLibrary);
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            ProjectLibraryChangeSubscription?.Dispose();
            ProjectEntityChangeSubscription?.Dispose();
        }

        /// <summary>
        ///     Action that is executed when the <see cref="IMocassinProjectLibrary" /> changes
        /// </summary>
        /// <param name="projectLibrary"></param>
        private void OnProjectLibraryChanged(IMocassinProjectLibrary projectLibrary)
        {
            SwitchLibraryEntityChangeSubscription(projectLibrary);
            OnProjectLibraryChangedInternal(projectLibrary);
        }

        /// <summary>
        ///     Switches the current library entity change subscription to the passed library or deletes the subscription if the
        ///     library is null
        /// </summary>
        /// <param name="projectLibrary"></param>
        private void SwitchLibraryEntityChangeSubscription(IMocassinProjectLibrary projectLibrary)
        {
            ProjectEntityChangeSubscription?.Dispose();
            ProjectEntityChangeSubscription = projectLibrary?.ModelChangedNotification.Subscribe(OnProjectContentChanged);
        }

        /// <summary>
        ///     Action that is executed when the content of the current <see cref="IMocassinProjectLibrary" /> changes
        /// </summary>
        /// <param name="unit"></param>
        private void OnProjectContentChanged(Unit unit)
        {
            OnProjectContentChangedInternal();
        }

        /// <summary>
        ///     Implementation dependent reaction to the change of the <see cref="IMocassinProjectLibrary" />
        /// </summary>
        /// <param name="newProjectLibrary"></param>
        protected virtual void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
        }

        /// <summary>
        ///     Implementation dependent reaction to the change of the entities in the open <see cref="IMocassinProjectLibrary" />
        /// </summary>
        protected virtual void OnProjectContentChangedInternal()
        {
        }

        /// <summary>
        ///     Creates a new <see cref="InfoMessage" /> and distributes it through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="shortInfo"></param>
        /// <param name="details"></param>
        protected void PushInfoMessage(string shortInfo, IEnumerable<string> details)
        {
            var message = new InfoMessage(this, shortInfo)
            {
                Details = details.ToList()
            };
            ProjectControl.PushMessageSystem.SendMessage(message);
        }

        /// <summary>
        ///     Creates a new <see cref="InfoMessage" /> which informs about a warning in the calling method and distributes it
        ///     through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="details"></param>
        /// <param name="callMemberName"></param>
        protected void PushInfoMessage(IEnumerable<string> details, [CallerMemberName] string callMemberName = default)
        {
            PushInfoMessage($"Message from {callMemberName}", details);
        }

        /// <summary>
        ///     Creates a new <see cref="InfoMessage" /> which informs about a warning in the calling method and distributes it
        ///     through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="callMemberName"></param>
        protected void PushInfoMessage(string detail, [CallerMemberName] string callMemberName = default)
        {
            PushInfoMessage(detail.AsSingleton(), callMemberName);
        }

        /// <summary>
        ///     Creates a new <see cref="WarningMessage" /> and distributes it through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="shortInfo"></param>
        /// <param name="details"></param>
        protected void PushWarningMessage(string shortInfo, IEnumerable<string> details)
        {
            var message = new WarningMessage(this, shortInfo)
            {
                Details = details.ToList()
            };
            ProjectControl.PushMessageSystem.SendMessage(message);
        }

        /// <summary>
        ///     Creates a new <see cref="WarningMessage" /> which informs about a warning in the calling method and distributes it
        ///     through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="details"></param>
        /// <param name="callMemberName"></param>
        protected void PushWarningMessage(IEnumerable<string> details, [CallerMemberName] string callMemberName = default)
        {
            PushWarningMessage($"Warning from {callMemberName}", details);
        }

        /// <summary>
        ///     Creates a new <see cref="WarningMessage" /> which informs about a warning in the calling method and distributes it
        ///     through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="callMemberName"></param>
        protected void PushWarningMessage(string detail, [CallerMemberName] string callMemberName = default)
        {
            PushWarningMessage(detail.AsSingleton(), callMemberName);
        }

        /// <summary>
        ///     Creates a new <see cref="ErrorMessage" /> and distributes it through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="shortInfo"></param>
        /// <param name="exception"></param>
        protected void PushErrorMessage(string shortInfo, Exception exception)
        {
            var message = new ErrorMessage(this, shortInfo)
            {
                Exception = exception
            };
            ProjectControl.PushMessageSystem.SendMessage(message);
        }

        /// <summary>
        ///     Creates a new <see cref="ErrorMessage" /> which informs about a failure in the calling method and distributes it
        ///     through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="callMemberName"></param>
        /// <param name="exception"></param>
        protected void PushErrorMessage(Exception exception, [CallerMemberName] string callMemberName = default)
        {
            PushErrorMessage($"Error from {callMemberName}", exception);
        }
    }
}