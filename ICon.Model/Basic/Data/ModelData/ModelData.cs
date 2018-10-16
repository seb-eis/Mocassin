using System;
using System.Reactive.Disposables;
using System.Reflection;

namespace ICon.Model.Basic
{
    /// <summary>
    ///     Base class for all model data objects that contain potentially critical data and support thread safe read/write
    ///     operations through disposables
    /// </summary>
    public abstract class ModelData
    {
        /// <summary>
        ///     The access lock to safely set and unset the read/write flags
        /// </summary>
        private object AccessLock { get; } = new object();

        /// <summary>
        ///     Boolean flag that indicates if a write operation is currently in progress
        /// </summary>
        public bool IsWriteInProgress { get; protected set; }

        /// <summary>
        ///     Boolean flag that indicates if active readers exist
        /// </summary>
        public bool IsReadInProgress => CurrentReaders != 0;

        /// <summary>
        ///     Count how many active reader instances exists, the data is blocked for writing as long as at least one non-disposed
        ///     reader exists
        /// </summary>
        public int CurrentReaders { get; protected set; }

        /// <summary>
        ///     Tries to get a disposable full access lock that restricts access for other sources until disposed, returns false if
        ///     currently in use
        /// </summary>
        /// <param name="locker"></param>
        /// <returns></returns>
        public bool TryGetFullLock(out IDisposable locker)
        {
            lock (AccessLock)
            {
                locker = null;
                if (IsWriteInProgress)
                    return false;

                if (IsReadInProgress)
                    return false;

                IsWriteInProgress = true;
                locker = Disposable.Create(() =>
                {
                    lock (AccessLock)
                    {
                        IsWriteInProgress = false;
                    }
                });
            }

            return true;
        }

        /// <summary>
        ///     Tries to get a disposable read only lock that blocks writing locks from other sources but allows reading until
        ///     disposed, returns false if writing is currently in progress
        /// </summary>
        /// <param name="locker"></param>
        /// <returns></returns>
        public bool TryGetReadingLock(out IDisposable locker)
        {
            lock (AccessLock)
            {
                locker = null;
                if (IsWriteInProgress)
                    return false;

                CurrentReaders++;
                locker = Disposable.Create(() =>
                {
                    lock (AccessLock)
                    {
                        CurrentReaders--;
                    }
                });
            }

            return true;
        }

        /// <summary>
        ///     Resets the data object to default construction conditions
        /// </summary>
        public abstract void ResetToDefault();

        /// <summary>
        ///     Get the read only interface as a general model data port
        /// </summary>
        /// <returns></returns>
        public abstract IModelDataPort GetModelDataPort();

        /// <summary>
        ///     Generic factory to create default state model data objects
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        public static TData CreateDefault<TData>() where TData : ModelData, new()
        {
            var obj = new TData();
            obj.ResetToDefault();
            return obj;
        }
    }

    /// <summary>
    ///     Generic abstract base class for all model data objects that supply a specific read only access port
    /// </summary>
    /// <typeparam name="TPort"></typeparam>
    public abstract class ModelData<TPort> : ModelData where TPort : IModelDataPort
    {
        /// <summary>
        ///     Get the read only access port for the model data object
        /// </summary>
        /// <returns></returns>
        public abstract TPort AsReadOnly();

        /// <inheritdoc />
        public override IModelDataPort GetModelDataPort()
        {
            return AsReadOnly();
        }

        /// <summary>
        ///     Replaces all index data by new containers of their type (Containers require a parameter-less constructor)
        /// </summary>
        protected void ResetAllIndexedData()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var propertyInfo in GetType().GetProperties(flags))
            {
                if (propertyInfo.GetCustomAttribute(typeof(IndexedModelDataAttribute)) is IndexedModelDataAttribute)
                    propertyInfo.SetValue(this, Activator.CreateInstance(propertyInfo.PropertyType));
            }
        }
    }
}