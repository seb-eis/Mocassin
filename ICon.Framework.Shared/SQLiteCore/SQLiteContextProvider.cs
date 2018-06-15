using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ICon.Framework.SQLiteCore
{
    /// <summary>
    /// Generic context provider that enables context building manipultaion
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public abstract class SQLiteContextProvider<T1> : ISQLiteContextProvider<T1> where T1 : SQLiteContext<T1>, new()
    {
        /// <summary>
        /// The default filepath for the context database
        /// </summary>
        public static String DefaultFilepath;

        /// <summary>
        /// The actually used filepath
        /// </summary>
        public String Filepath { get; }

        /// <summary>
        /// New context provider utilizing the default filepath
        /// </summary>
        protected SQLiteContextProvider()
        {
            Filepath = DefaultFilepath;
        }

        /// <summary>
        /// Creates a context provider with the specified filepath (Checks if filepath exists)
        /// </summary>
        /// <param name="filepath"></param>
        protected SQLiteContextProvider(String filepath)
        {
            if (File.Exists(filepath) == false)
            {
                throw new ArgumentException(paramName: nameof(filepath), message: "The provided database file does not exist");
            }
            Filepath = filepath ?? throw new ArgumentNullException(nameof(filepath));
        }

        /// <summary>
        /// Creates a new database context
        /// </summary>
        /// <param name="optionsBuilderParameterString"></param>
        /// <returns></returns>
        public T1 CreateContext()
        {
            return new T1().CreateNewContext("Filename=" + Filepath);
        }
    }
}
