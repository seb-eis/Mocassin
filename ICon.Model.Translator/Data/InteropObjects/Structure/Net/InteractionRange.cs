using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    public class InteractionRange : InteropObject<CInteractionRange>
    {
        public InteractionRange()
        {
        }

        public InteractionRange(CInteractionRange structure) : base(structure)
        {
        }
    }
}
