using System;
using System.Collections.Generic;
using System.Reactive;
using ICon.Framework.Events;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for implementations of value setters that enbale manipulation of model data that does not require complex validation
    /// </summary>
    public abstract class ValueSetter : IValueSetter
    {
        /// <summary>
        /// Observable for subscriptions to value pushed to the model data
        /// </summary>
        public IObservable<Unit> WhenValuesPushed => OnValuesPushed.AsObservable();

        /// <summary>
        /// Observable for subscriptions to lokal value pushed in the setter
        /// </summary>
        public IObservable<Unit> WhenValueChanged => OnValueChanged.AsObservable();

        /// <summary>
        /// Event provider for the subject of locally changed values
        /// </summary>
        protected ReactiveEvent<Unit> OnValueChanged { get; set; }

        /// <summary>
        /// Event provider for the subject of data pushes to the model
        /// </summary>
        protected ReactiveEvent<Unit> OnValuesPushed { get; set; }

        /// <summary>
        /// Create new value setter and initialize the event providers
        /// </summary>
        protected ValueSetter()
        {
            OnValueChanged = new ReactiveEvent<Unit>();
            OnValuesPushed = new ReactiveEvent<Unit>();
        }

        /// <summary>
        /// Push the locally stored data into the model system
        /// </summary>
        public abstract void PushData();
    }
}
