using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ICon.Framework.SQLiteCore;

namespace ICon.Symmetry.SpaceGroups
{
    /// <summary>
    /// Provider for space group context
    /// </summary>
    public class SpaceGroupContextProvider : SQLiteContextProvider<SpaceGroupContext>
    {
        /// <summary>
        ///  The default filepath to the space group database
        /// </summary>
        public static new String DefaultFilepath = "C:/Users/hims-user/source/repos/ICon.Project/ICon.Framework.Symmetry/SpaceGroups/SpaceGroups.db";

        /// <summary>
        /// New context provider utilizing the default filepath
        /// </summary>
        public SpaceGroupContextProvider() : base(DefaultFilepath)
        {
        }

        /// <summary>
        /// Creates a context provider with the specified filepath (Checks if filepath exists)
        /// </summary>
        /// <param name="filepath"></param>
        public SpaceGroupContextProvider(String filepath) : base(filepath)
        {
        }

        /// <summary>
        /// Factory method to create a new space group context
        /// </summary>
        /// <returns></returns>
        public SpaceGroupContext NewContext()
        {
            return new SpaceGroupContext("Filename=" + DefaultFilepath);
        }
    }
}
