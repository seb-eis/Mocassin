using System;
using System.IO;

namespace Mocassin.Framework.SQLiteCore
{
    /// <inheritdoc />
    public abstract class SqLiteContextProvider<T1> : ISqLiteContextProvider<T1> where T1 : SqLiteContext<T1>
    {
        /// <summary>
        ///     The default filepath for the context database
        /// </summary>
        public abstract string DefaultFilepath { get; }

        /// <summary>
        ///     The actually used filepath
        /// </summary>
        public string Filepath { get; }

        /// <summary>
        ///     Creates a context provider with the specified filepath (Checks if filepath exists)
        /// </summary>
        /// <param name="filepath"></param>
        protected SqLiteContextProvider(string filepath)
        {
            Filepath = filepath ?? throw new ArgumentNullException(nameof(filepath));
        }

        /// <inheritdoc />
        public T1 CreateContext()
        {
            return SqLiteContext.OpenDatabase<T1>(Filepath ?? DefaultFilepath);
        }
    }
}