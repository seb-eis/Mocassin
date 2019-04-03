using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Mocassin.Framework.Extensions;
using Mocassin.Framework.Messaging;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Helper;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.Base.ViewModels
{
    /// <summary>
    ///     Base <see cref="ViewModelBase" /> implementation for primary controls with access to the components of the
    ///     <see cref="IMocassinProjectControl" />
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
        ///     Get the main <see cref="IMocassinProjectControl" /> that this sub control is connected to
        /// </summary>
        public IMocassinProjectControl ProjectControl { get; }

        /// <summary>
        ///     Create new <see cref="PrimaryControlViewModel" /> that is connected to the passed
        ///     <see cref="IMocassinProjectControl" />
        /// </summary>
        /// <param name="projectControl"></param>
        protected PrimaryControlViewModel(IMocassinProjectControl projectControl)
        {
            ProjectControl = projectControl ?? throw new ArgumentNullException(nameof(projectControl));
            ProjectLibraryChangeSubscription = projectControl.ProjectLibraryChangeNotification
                .Subscribe(OnProjectLibraryChanged);

            if (ProjectControl.OpenProjectLibrary != null) OnProjectLibraryChanged(projectControl.OpenProjectLibrary);
        }

        /// <summary>
        ///     Action that is executed when the <see cref="IMocassinProjectLibrary" /> changes
        /// </summary>
        /// <param name="newProjectLibrary"></param>
        private void OnProjectLibraryChanged(IMocassinProjectLibrary newProjectLibrary)
        {
            ProjectEntityChangeSubscription?.Dispose();
            ProjectEntityChangeSubscription = newProjectLibrary?.StateChangedNotification.Subscribe(OnProjectContentChanged);
            OnProjectLibraryChangedInternal(newProjectLibrary);
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

        /// <inheritdoc />
        public virtual void Dispose()
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
            ProjectControl.PushMessageSystem.SendMessage(message);
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
            ProjectControl.PushMessageSystem.SendMessage(message);
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
            ProjectControl.PushMessageSystem.SendMessage(message);
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

        /// <summary>
        ///     Searches the primary and plugin <see cref="Assembly" /> instances for marked <see cref="ProjectControlCommand" />
        ///     types and creates a <see cref="IReadOnlyDictionary{TKey,TValue}" /> using the provided
        ///     <see cref="Func{T, TResult}" /> and the <see cref="IMocassinProjectControl" /> the
        ///     <see cref="PrimaryControlViewModel" /> is linked to
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public IReadOnlyDictionary<TValue, ICommand> CreateCommandLookupDictionary<TAttribute, TValue>(Func<TAttribute, TValue> selector)
            where TAttribute : Attribute
        {
            var dictionary = Assembly.GetAssembly(typeof(ProjectControlCommand)).AsSingleton()
                .Concat(ProjectControl.PluginAssemblies)
                .SelectMany(x => x.MakeAttributedInstances<ProjectControlCommand, TAttribute>(ProjectControl))
                .ToDictionary(y => selector(y.Value), z => (ICommand) z.Key);

            return dictionary;
        }
    }
}