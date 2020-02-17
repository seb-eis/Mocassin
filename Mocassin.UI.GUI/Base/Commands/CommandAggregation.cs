using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Mocassin.UI.Base.Commands
{
    /// <summary>
    ///     Generic <see cref="ICommand" /> wrapper to form a single <see cref="ICommand" /> interface for multiple unordered commands
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class CommandAggregation<T> : Command, IList<T> where T : ICommand
    {
        /// <summary>
        ///     The <see cref="ICollection{T}" /> of sub commands
        /// </summary>
        private readonly IList<T> commandList;

        /// <inheritdoc />
        public int Count => commandList.Count;

        /// <inheritdoc />
        public bool IsReadOnly => commandList.IsReadOnly;

        /// <inheritdoc />
        public T this[int index]
        {
            get => commandList[index];
            set => commandList[index] = value;
        }

        /// <summary>
        ///     Creates a new <see cref="CommandAggregation{T}" /> with the passed parameters
        /// </summary>
        /// <param name="commands"></param>
        public CommandAggregation(params T[] commands)
        {
            if (commands == null) throw new ArgumentNullException(nameof(commands));

            commandList = commands.ToList();
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return commandList.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) commandList).GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(T item)
        {
            commandList.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            commandList.Clear();
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            return commandList.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            commandList.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            return commandList.Remove(item);
        }

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            return commandList.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, T item)
        {
            commandList.Insert(index, item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            commandList.RemoveAt(index);
        }

        /// <inheritdoc />
        public override void Execute(object parameter)
        {
            foreach (var command in commandList) command.Execute(parameter);
        }

        /// <inheritdoc />
        public override bool CanExecute(object parameter)
        {
            return commandList.All(x => x.CanExecute(parameter));
        }
    }
}