using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    /// <summary>
    /// 4D encoded environment position that carries a 4D relative vector information 
    /// </summary>
    public class EnvironmentPosition : EntityBase
    {
        /// <summary>
        /// The environment shell entity id
        /// </summary>
        [ForeignKey(nameof(EnvironmentShell))]
        public int EnvironmentShellId { get; set; }

        /// <summary>
        /// Navigation property for the environment shell this position belongs to
        /// </summary>
        public EnvironmentShell EnvironmentShell { get; set; }

        /// <summary>
        /// The linear index of this position in the environment position list
        /// </summary>
        public int IndexInEnvironment { get; set; }

        /// <summary>
        /// Relative vector component in A direction
        /// </summary>
        public int RelativeA { get; set; }

        /// <summary>
        /// Relative vector component in B direction
        /// </summary>
        public int RelativeB { get; set; }

        /// <summary>
        /// Relative vector component in C direction
        /// </summary>
        public int RelativeC { get; set; }

        /// <summary>
        /// Relative vector component in D direction
        /// </summary>
        public int RelativeD { get; set; }
    }
}
