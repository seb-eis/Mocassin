using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Base class for a collection of disposables that handles storage and disposing of multiple disposbales
    /// </summary>
    public class DisposableCollection : IDisposable
    {
        /// <summary>
        /// The list of disposables
        /// </summary>
        protected List<IDisposable> Disposables { get; set; }

        /// <summary>
        /// Flag if the container contents are disposed
        /// </summary>
        public Boolean IsDisposed => Disposables.Count == 0;

        /// <summary>
        /// Creates new disposable collection with an initial list of disposables
        /// </summary>
        /// <param name="disposables"></param>
        public DisposableCollection(List<IDisposable> disposables)
        {
            Disposables = disposables ?? throw new ArgumentNullException(nameof(disposables));
        }

        /// <summary>
        /// Creates new disposables collection with empty disposable list
        /// </summary>
        public DisposableCollection()
        {
            Disposables = new List<IDisposable>();
        }

        /// <summary>
        /// Adds a new disposable to the collection
        /// </summary>
        /// <param name="disposable"></param>
        public void Add(IDisposable disposable)
        {
            Disposables.Add(disposable);
        }

        /// <summary>
        /// Adds multiple disposables by enumerable
        /// </summary>
        /// <param name="disposables"></param>
        public void Add(IEnumerable<IDisposable> disposables)
        {
            if (disposables == null)
            {
                throw new ArgumentNullException(nameof(disposables));
            }
            foreach (IDisposable disposable in disposables)
            {
                Add(disposable);
            }
        }

        /// <summary>
        /// Disposes all stored disposables and clears the list
        /// </summary>
        public void Dispose()
        {
            foreach (IDisposable disposable in Disposables)
            {
                disposable?.Dispose();
            }
            Disposables.Clear();
        }
    }
}
