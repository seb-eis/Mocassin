﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    public class JobInfo : InteropObject<CJobInfo>
    {
        public JobInfo()
        {
        }

        public JobInfo(CJobInfo structure) : base(structure)
        {
        }
    }
}
