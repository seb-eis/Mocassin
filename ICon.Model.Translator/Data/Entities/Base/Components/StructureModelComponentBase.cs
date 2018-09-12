using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ICon.Model.Translator
{
    public class StructureModelComponentBase : InteropEntityBase
    {
        private static IList<InteropStateChangeAction> stateChangeDelegates;

        protected override IList<InteropStateChangeAction> StateChangeActions
        {
            get => stateChangeDelegates;
            set => stateChangeDelegates = value;
        }

        [ForeignKey(nameof(StructureModel))]
        public int StructureModelId { get; set; }

        public StructureModel StructureModel { get; set; }
    }
}
