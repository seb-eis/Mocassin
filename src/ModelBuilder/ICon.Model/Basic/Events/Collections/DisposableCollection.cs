using System;
using System.Collections.Generic;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Base class for a collection of disposables that handles storage and disposing of multiple disposables
    /// </summary>
    public class DisposableCollection : IDisposable
    {
        /// <summary>
        ///     The list of disposables
        /// </summary>
        protected ICollection<IDisposable> Disposables { get; set; }

        /// <summary>
        ///     Flag if the container contents are disposed
        /// </summary>
        public bool IsDisposed => Disposables.Count == 0;

        /// <summary>
        ///     Creates new disposable collection with an initial collection of disposables
        /// </summary>
        /// <param name="disposables"></param>
        public DisposableCollection(ICollection<IDisposable> disposables)
        {
            Disposables = disposables ?? throw new ArgumentNullException(nameof(disposables));
        }

        /// <summary>
        ///     Creates new disposables collection with empty disposable list
        /// </summary>
        public DisposableCollection()
        {
            Disposables = new List<IDisposable>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var disposable in Disposables)
                disposable?.Dispose();

            Disposables.Clear();
        }

        /// <summary>
        ///     Adds a new disposable to the collection
        /// </summary>
        /// <param name="disposable"></param>
        public void Add(IDisposable disposable)
        {
            Disposables.Add(disposable);
        }

        /// <summary>
        ///     Adds multiple disposables by enumerable
        /// </summary>
        /// <param name="disposables"></param>
        public void Add(IEnumerable<IDisposable> disposables)
        {
            if (disposables == null)
                throw new ArgumentNullException(nameof(disposables));

            foreach (var disposable in disposables)
                Add(disposable);
        }
    }
}