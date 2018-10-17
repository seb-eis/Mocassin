using System;
using System.Reactive;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Represents a value setter adapter that enables save manipulation of model data that does not require validation
    /// </summary>
    public interface IValueSetter
    {
        /// <summary>
        ///     Observable to subscribe for reactions when the setter pushed data
        /// </summary>
        IObservable<Unit> WhenValuesPushed { get; }

        /// <summary>
        ///     Observable to subscribe for reactions when a temporary value changes
        /// </summary>
        IObservable<Unit> WhenValueChanged { get; }

        /// <summary>
        ///     Pushes the stored value manipulations into the model data
        /// </summary>
        /// <returns></returns>
        void PushData();
    }
}