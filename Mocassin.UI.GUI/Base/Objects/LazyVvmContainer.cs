using System;
using System.Windows.Controls;
using Mocassin.UI.GUI.Base.ViewModels;

namespace Mocassin.UI.GUI.Base.Objects
{
    /// <summary>
    ///     Container for passing <see cref="VvmContainer" /> instances that are created lazy
    /// </summary>
    public class LazyVvmContainer : ViewModelBase
    {
        protected readonly Lazy<VvmContainer> lazyContainer;

        /// <summary>
        ///     Get the name of the container
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     Get the lazy initialized <see cref="VvmContainer" />
        /// </summary>
        public VvmContainer Value => lazyContainer.Value;

        /// <summary>
        ///     Creates a new named <see cref="LazyVvmContainer" /> from delegates for <see cref="UserControl" /> and
        ///     <see cref="ViewModelBase" />
        /// </summary>
        /// <param name="viewFactory"></param>
        /// <param name="viewModelFactory"></param>
        /// <param name="name"></param>
        public LazyVvmContainer(Func<UserControl> viewFactory, Func<ViewModelBase> viewModelFactory, string name)
        {
            Name = name;
            lazyContainer = new Lazy<VvmContainer>(() => new VvmContainer(InvokeFactory(viewFactory), InvokeFactory(viewModelFactory), name));
        }

        /// <summary>
        ///     Invokes the factory methods and ensures its done on the app thread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        protected virtual T InvokeFactory<T>(Func<T> factory) => ExecuteOnAppThread(factory);
    }
}