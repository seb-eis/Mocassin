﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    public class MmcJobHeader : InteropObject<CMmcJobHeader>
    {
        public MmcJobHeader()
        {
        }

        public MmcJobHeader(CMmcJobHeader structure) : base(structure)
        {
        }
    }
}
