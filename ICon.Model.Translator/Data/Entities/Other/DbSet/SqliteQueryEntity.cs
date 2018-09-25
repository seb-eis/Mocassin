using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Sqlite query entity for storing the data loading sql statements used in the simulator within the interop database itself
    /// </summary>
    [XmlRoot("SQLite3")]
    public class SqliteQueryEntity : EntityBase
    {
        /// <summary>
        /// The indentification string for lookup
        /// </summary>
        [XmlAttribute("Id")]
        public string Identification { get; set; }

        /// <summary>
        /// The query string that loads the required data from the database
        /// </summary>
        [XmlText]
        public string QueryString { get; set; }
    }
}
