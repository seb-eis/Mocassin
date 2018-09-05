using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class EnergyModelComponentBase : InteropEntityBase
    {
        [ForeignKey(nameof(EnergyModel))]
        public int EnergyModelId { get; set; }

        public EnergyModel EnergyModel { get; set; }
    }
}

