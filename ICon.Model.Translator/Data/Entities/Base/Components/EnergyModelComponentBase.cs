using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Mocassin.Model.Translator
{
    public class EnergyModelComponentBase : InteropEntityBase
    {
        private static IList<InteropStateChangeAction> stateChangeDelegates;

        protected override IList<InteropStateChangeAction> StateChangeActions
        {
            get => stateChangeDelegates;
            set => stateChangeDelegates = value;
        }

        [ForeignKey(nameof(EnergyModel))]
        public int EnergyModelId { get; set; }

        public EnergyModel EnergyModel { get; set; }
    }
}

