using System;
using System.Threading.Tasks;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Generic cached object source that encapsulates data and recalculation instruction into an 'on-demand' data supplier
    /// </summary>
    public class CachedObjectSource<T1> : ICachedObjectSource
    {
        /// <summary>
        ///     The access lock object to protect from multiple induced data recalculation
        /// </summary>
        private object AccessLock { get; } = new object();

        /// <inheritdoc />
        public bool IsDeprecated { get; protected set; }

        /// <summary>
        ///     The data creation delegate that is triggered each time data is deprecated or null
        /// </summary>
        protected Func<T1> DataCreator { get; set; }

        /// <summary>
        ///     The cached data object
        /// </summary>
        protected T1 Data { get; set; }

        /// <inheritdoc />
        public Delegate FactoryDelegate => DataCreator;

        /// <summary>
        ///     Create new auto cached data object that is by default deprecated
        /// </summary>
        protected CachedObjectSource()
        {
            lock (AccessLock)
            {
                IsDeprecated = true;
            }
        }

        /// <summary>
        ///     Creates new auto cached data object that uses the provided creation function
        /// </summary>
        /// <param name="dataCreator"></param>
        public CachedObjectSource(Func<T1> dataCreator)
            : this()
        {
            DataCreator = dataCreator ?? throw new ArgumentNullException(nameof(dataCreator));
        }

        /// <inheritdoc />
        public void Clear()
        {
            lock (AccessLock)
            {
                Data = default;
                IsDeprecated = true;
            }
        }

        /// <inheritdoc />
        public void ClearIfDeprecated()
        {
            lock (AccessLock)
            {
                if (IsDeprecated) Clear();
            }
        }

        /// <inheritdoc />
        public object GetValue() => GetData();

        /// <inheritdoc />
        public Task<object> GetValueAsync() => Task.Run(GetValue);

        /// <inheritdoc />
        public bool Equals(ICachedObjectSource other) => other != null && FactoryDelegate.Equals(other.FactoryDelegate);

        /// <inheritdoc />
        public void MarkAsDeprecated()
        {
            lock (AccessLock)
            {
                IsDeprecated = true;
            }
        }

        /// <summary>
        ///     Get the data object, triggers update if data is deprecated/null
        /// </summary>
        /// <returns></returns>
        public T1 GetData()
        {
            lock (AccessLock)
            {
                if (IsDeprecated) Recalculate();

                return Data;
            }
        }

        /// <summary>
        ///     Start data object retrieval as a new task, triggers data update if deprecated flag is set
        /// </summary>
        /// <returns></returns>
        public Task<T1> GetDataAsync() => Task.Run(GetData);

        /// <summary>
        ///     Triggers update function and sets the deprecated flag to false
        /// </summary>
        protected void Recalculate()
        {
            Data = DataCreator();
            IsDeprecated = false;
        }
    }
}