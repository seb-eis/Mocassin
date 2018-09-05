using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class StructureModelComponentBase : InteropEntityBase
    {
        [ForeignKey(nameof(StructureModel))]
        public int StructureModelId { get; set; }

        public StructureModel StructureModel { get; set; }
    }
}
