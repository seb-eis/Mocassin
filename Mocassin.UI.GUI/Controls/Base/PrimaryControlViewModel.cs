﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Mocassin.Framework.Messaging;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.Base
{
    /// <summary>
    ///     Base <see cref="ViewModel" /> implementation for primary controls with access to the components of the
    ///     <see cref="IMocassinProjectControl" />
    /// </summary>
    public abstract class PrimaryControlViewModel : ViewModel, IDisposable
    {
        /// <summary>
        ///     Get the <see cref="IDisposable" /> subscription to the <see cref="IMocassinProjectLibrary" /> changes
        /// </summary>
        private IDisposable ProjectLibraryChangeSubscription { get; }

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
            ProjectLibraryChangeSubscription = mainProjectControl.LibraryChangeNotification.Subscribe(OnProjectLibraryChanged);
        }

        /// <summary>
        ///     Action that is executed when the <see cref="IMocassinProjectLibrary" /> changes
        /// </summary>
        /// <param name="newProjectLibrary"></param>
        protected void OnProjectLibraryChanged(IMocassinProjectLibrary newProjectLibrary)
        {
            OnProjectLibraryChangedInternal(newProjectLibrary);
        }

        /// <summary>
        ///     Implementation dependent reaction to the change of the <see cref="IMocassinProjectLibrary" />
        /// </summary>
        /// <param name="newProjectLibrary"></param>
        protected virtual void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
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
            SendInfoMessage($"Info\t@ {callMemberName}", details);
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
            SendWarningMessage($"Warning\t@ {callMemberName}", details);
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
            SendErrorMessage($"Error\t@ {callMemberName}", exception);
        }
    }
}