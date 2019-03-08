using System;
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
        protected IMocassinProjectControl MainProjectControl { get; }

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
            if (MainProjectControl.OpenProjectLibrary == newProjectLibrary) return;
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
    }
}