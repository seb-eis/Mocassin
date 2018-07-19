using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ICon.Model.Translator
{
    /// <summary>
    /// Mccs database context for encoded simulation data and result entities
    /// </summary>
    public class MccsDatabaseContext : DbContext
    {
        /// <summary>
        /// The database set of mccs package entities
        /// </summary>
        DbSet<MccsPackage> MccsPackages { get; set; }

        /// <summary>
        /// The database set of mccs parent entities
        /// </summary>
        DbSet<MccsParent> MccsParents { get; set; }

        /// <summary>
        /// The database set of mccs meta component entities
        /// </summary>
        DbSet<MccsMetaEntity> MccsMetaEntities { get; set; }

        /// <summary>
        /// The database set of mccs structure component entities
        /// </summary>
        DbSet<MccsStructureEntity> MccsStructureEntities { get; set; }

        /// <summary>
        /// The database set of mccs transition component entities
        /// </summary>
        DbSet<MccsTransitionEntity> MccsTransitionEntities { get; set; }

        /// <summary>
        /// The database set of mccs energy component entities
        /// </summary>
        DbSet<MccsEnergyEntity> MccsEnergyEntities { get; set; }

        /// <summary>
        /// The database set of mccs job entities
        /// </summary>
        DbSet<MccsJob> MccsJobs { get; set; }

        /// <summary>
        /// The database set of mccs job result entities
        /// </summary>
        DbSet<MccsJobResult> MccsJobResults { get; set; }
    }
}
