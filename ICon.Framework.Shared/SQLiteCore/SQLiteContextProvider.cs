using System;
using System.IO;

namespace ICon.Framework.SQLiteCore
{
    /// <inheritdoc />
    public abstract class SqLiteContextProvider<T1> : ISqLiteContextProvider<T1> where T1 : SqLiteContext<T1>, new()
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
        ///     New context provider utilizing the default filepath
        /// </summary>
        protected SqLiteContextProvider()
        {

        }

        /// <summary>
        ///     Creates a context provider with the specified filepath (Checks if filepath exists)
        /// </summary>
        /// <param name="filepath"></param>
        protected SqLiteContextProvider(string filepath)
        {
            if (File.Exists(filepath) == false)
                throw new ArgumentException(paramName: nameof(filepath), message: "The provided database file does not exist");

            Filepath = filepath ?? throw new ArgumentNullException(nameof(filepath));
        }

        /// <inheritdoc />
        public T1 CreateContext()
        {
            return new T1().CreateNewContext("Filename=" + (Filepath ?? DefaultFilepath));
        }
    }
}