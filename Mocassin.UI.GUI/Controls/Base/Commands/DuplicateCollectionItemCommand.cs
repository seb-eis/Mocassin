using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mocassin.Framework.Extensions;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.GUI.Controls.Base.Commands
{
    /// <summary>
    ///     Async command to duplicate a <see cref="ProjectObjectGraph" /> contained in a collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DuplicateCollectionItemCommand<T> : AsyncCommand<T> where T : IDuplicable<T>
    {
        /// <summary>
        ///     Get the <see cref="CollectionControlViewModel{T}" /> that is targeted
        /// </summary>
        private CollectionControlViewModel<T> CollectionViewModel { get; }

        /// <summary>
        ///     Get or set a <see cref="Func{TResult}" /> that provides the count
        /// </summary>
        public Func<int> CountProvider { get; set; }

        /// <inheritdoc />
        public DuplicateCollectionItemCommand(CollectionControlViewModel<T> collectionViewModel)
        {
            CollectionViewModel = collectionViewModel ?? throw new ArgumentNullException(nameof(collectionViewModel));
        }

        /// <inheritdoc />
        public override Task ExecuteAsync(T parameter)
        {
            void TaskFunction()
            {
                var count = CountProvider?.Invoke() ?? 1;
                count = count > 0 ? count : 1;
                CollectionViewModel.DuplicateItem(parameter, x => ((IDuplicable<T>) x).Duplicate(), count);
            }

            return Task.Run(TaskFunction);
        }

        /// <inheritdoc />
        public override bool CanExecuteInternal(T parameter)
        {
            return CollectionViewModel.DataCollection != null && CollectionViewModel.DataCollection.Contains(parameter);
        }
    }
}