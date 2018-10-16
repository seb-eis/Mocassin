using System;
using System.Reactive;
using ICon.Framework.Events;

namespace ICon.Model.Basic
{
    /// <inheritdoc />
    public abstract class ValueSetter : IValueSetter
    {
        /// <inheritdoc />
        public IObservable<Unit> WhenValuesPushed => OnValuesPushed.AsObservable();

        /// <inheritdoc />
        public IObservable<Unit> WhenValueChanged => OnValueChanged.AsObservable();

        /// <summary>
        ///     Event provider for the subject of locally changed values
        /// </summary>
        protected ReactiveEvent<Unit> OnValueChanged { get; set; }

        /// <summary>
        ///     Event provider for the subject of data pushes to the model
        /// </summary>
        protected ReactiveEvent<Unit> OnValuesPushed { get; set; }

        /// <summary>
        ///     Create new value setter and initialize the event providers
        /// </summary>
        protected ValueSetter()
        {
            OnValueChanged = new ReactiveEvent<Unit>();
            OnValuesPushed = new ReactiveEvent<Unit>();
        }

        /// <inheritdoc />
        public abstract void PushData();
    }
}