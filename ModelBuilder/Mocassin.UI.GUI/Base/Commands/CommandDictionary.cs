using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace Mocassin.UI.Base.Commands
{
    /// <summary>
    ///     Adapter that supplies <see cref="Command{T}" /> implementation where the parameter identifies the parameterless
    ///     <see cref="Command" /> to execute
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TCommand"></typeparam>
    public class CommandDictionary<TKey, TCommand> : Command<TKey>, IDictionary<TKey, TCommand> where TCommand : ICommand
    {
        /// <summary>
        ///     Get the <see cref="IDictionary{TKey,TValue}" /> of commands
        /// </summary>
        private IDictionary<TKey, TCommand> Dictionary { get; }

        /// <inheritdoc />
        public int Count => Dictionary.Count;

        /// <inheritdoc />
        public bool IsReadOnly => Dictionary.IsReadOnly;

        /// <inheritdoc />
        public TCommand this[TKey key]
        {
            get => Dictionary[key];
            set => Dictionary[key] = value;
        }

        /// <inheritdoc />
        public ICollection<TKey> Keys => Dictionary.Keys;

        /// <inheritdoc />
        public ICollection<TCommand> Values => Dictionary.Values;

        /// <inheritdoc />
        public CommandDictionary()
        {
            Dictionary = new Dictionary<TKey, TCommand>();
        }

        /// <summary>
        ///     Creates new <see cref="CommandDictionary{TKey,TCommand}" /> with the passed <see cref="IDictionary{TKey,TValue}" />
        /// </summary>
        /// <param name="values"></param>
        public CommandDictionary(IDictionary<TKey, TCommand> values)
        {
            Dictionary = values ?? throw new ArgumentNullException(nameof(values));
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<TKey, TCommand>> GetEnumerator() => Dictionary.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) Dictionary).GetEnumerator();

        /// <inheritdoc />
        public void Add(KeyValuePair<TKey, TCommand> item)
        {
            Dictionary.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            Dictionary.Clear();
        }

        /// <inheritdoc />
        public bool Contains(KeyValuePair<TKey, TCommand> item) => Dictionary.Contains(item);

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<TKey, TCommand>[] array, int arrayIndex)
        {
            Dictionary.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(KeyValuePair<TKey, TCommand> item) => Dictionary.Remove(item);

        /// <inheritdoc />
        public bool ContainsKey(TKey key) => Dictionary.ContainsKey(key);

        /// <inheritdoc />
        public void Add(TKey key, TCommand value)
        {
            Dictionary.Add(key, value);
        }

        /// <inheritdoc />
        public bool Remove(TKey key) => Dictionary.Remove(key);

        /// <inheritdoc />
        public bool TryGetValue(TKey key, out TCommand value) => Dictionary.TryGetValue(key, out value);

        /// <inheritdoc />
        public override void Execute(TKey parameter)
        {
            Dictionary[parameter].Execute(null);
        }

        /// <inheritdoc />
        public override bool CanExecute(TKey parameter) => Dictionary.TryGetValue(parameter, out var relayCommand) && relayCommand.CanExecute(null);
    }
}