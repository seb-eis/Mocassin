using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Mocassin.Model.Translator
{
    public class TransitionModelComponentBase : InteropEntityBase
    {
        private static IList<InteropStateChangeAction> stateChangeDelegates;

        protected override IList<InteropStateChangeAction> StateChangeActions
        {
            get => stateChangeDelegates;
            set => stateChangeDelegates = value;
        }

        [ForeignKey(nameof(TransitionModel))]
        public int TransitionModelId { get; set; }

        public TransitionModel TransitionModel { get; set; }
    }
}
