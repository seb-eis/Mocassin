using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class TransitionModelComponentBase : InteropEntityBase
    {
        [ForeignKey(nameof(TransitionModel))]
        public int TransitionModelId { get; set; }

        public TransitionModel TransitionModel { get; set; }
    }
}
