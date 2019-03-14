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
    ///     Base <see cref="ViewModel" /> implementation for primary controls with access to the components of the
    ///     <see cref="IMocassinProjectControl" />
    /// </summary>
    public abstract class PrimaryControlViewModel : ViewModel, IDisposable
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
        ///     Get the main <see cref="IMocassinProjectControl" /> that this sub control is connected to
        /// </summary>
        public IMocassinProjectControl MainProjectControl { get; }

        /// <summary>
        ///     Create new <see cref="PrimaryControlViewModel" /> that is connected to the passed
        ///     <see cref="IMocassinProjectControl" />
        /// </summary>
        /// <param name="mainProjectControl"></param>
        protected PrimaryControlViewModel(IMocassinProjectControl mainProjectControl)
        {
            MainProjectControl = mainProjectControl ?? throw new ArgumentNullException(nameof(mainProjectControl));
            ProjectLibraryChangeSubscription = mainProjectControl.ProjectLibraryChangeNotification
                .Subscribe(OnProjectLibraryChanged);
        }

        /// <summary>
        ///     Action that is executed when the <see cref="IMocassinProjectLibrary" /> changes
        /// </summary>
        /// <param name="newProjectLibrary"></param>
        private void OnProjectLibraryChanged(IMocassinProjectLibrary newProjectLibrary)
        {
            ProjectEntityChangeSubscription = newProjectLibrary?.StateChangedNotification.Subscribe(OnProjectContentChanged);
            OnProjectLibraryChangedInternal(newProjectLibrary);
        }

        /// <summary>
        ///     Action that is executed when the content of the current <see cref="IMocassinProjectLibrary" /> changes
        /// </summary>
        /// <param name="unit"></param>
        private void OnProjectContentChanged(Unit unit)
        {
            ProjectEntityChangeSubscription?.Dispose();
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

        /// <inheritdoc />
        public void Dispose()
        {
            ProjectLibraryChangeSubscription?.Dispose();
        }

        /// <summary>
        ///     Creates a new <see cref="InfoMessage" /> and distributes it through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="shortInfo"></param>
        /// <param name="details"></param>
        protected void SendInfoMessage(string shortInfo, IEnumerable<string> details)
        {
            var message = new InfoMessage(this, shortInfo)
            {
                Details = details.ToList()
            };
            MainProjectControl.PushMessageSystem.SendMessage(message);
        }

        /// <summary>
        ///     Creates a new <see cref="InfoMessage" /> which informs about a warning in the calling method and distributes it
        ///     through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="details"></param>
        /// <param name="callMemberName"></param>
        protected void SendCallInfoMessage(IEnumerable<string> details, [CallerMemberName] string callMemberName = default)
        {
            SendInfoMessage($"Message @ {callMemberName}", details);
        }

        /// <summary>
        ///     Creates a new <see cref="InfoMessage" /> which informs about a warning in the calling method and distributes it
        ///     through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="detail"></param>
        /// <param name="callMemberName"></param>
        protected void SendCallInfoMessage(string detail, [CallerMemberName] string callMemberName = default)
        {
            SendCallInfoMessage(detail.AsSingleton(), callMemberName);
        }

        /// <summary>
        ///     Creates a new <see cref="WarningMessage" /> and distributes it through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="shortInfo"></param>
        /// <param name="details"></param>
        protected void SendWarningMessage(string shortInfo, IEnumerable<string> details)
        {
            var message = new WarningMessage(this, shortInfo)
            {
                Details = details.ToList()
            };
            MainProjectControl.PushMessageSystem.SendMessage(message);
        }

        /// <summary>
        ///     Creates a new <see cref="WarningMessage" /> which informs about a warning in the calling method and distributes it
        ///     through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="details"></param>
        /// <param name="callMemberName"></param>
        protected void SendCallWarningMessage(IEnumerable<string> details, [CallerMemberName] string callMemberName = default)
        {
            SendWarningMessage($"Warning @ {callMemberName}", details);
        }

        /// <summary>
        ///     Creates a new <see cref="WarningMessage" /> which informs about a warning in the calling method and distributes it
        ///     through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="detais"></param>
        /// <param name="callMemberName"></param>
        protected void SendCallWarningMessage(string detail, [CallerMemberName] string callMemberName = default)
        {
            SendCallWarningMessage(detail.AsSingleton(), callMemberName);
        }

        /// <summary>
        ///     Creates a new <see cref="ErrorMessage" /> and distributes it through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="shortInfo"></param>
        /// <param name="exception"></param>
        protected void SendErrorMessage(string shortInfo, Exception exception)
        {
            var message = new ErrorMessage(this, shortInfo)
            {
                Exception = exception
            };
            MainProjectControl.PushMessageSystem.SendMessage(message);
        }

        /// <summary>
        ///     Creates a new <see cref="ErrorMessage" /> which informs about a failure in the calling method and distributes it
        ///     through the main <see cref="IPushMessageSystem" />
        /// </summary>
        /// <param name="callMemberName"></param>
        /// <param name="exception"></param>
        protected void SendCallErrorMessage(Exception exception, [CallerMemberName] string callMemberName = default)
        {
            SendErrorMessage($"Error @ {callMemberName}", exception);
        }
    }
}