using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    public class JobInfo : InteropObject<C_JobInfo>
    {
        public JobInfo()
        {
        }

        public JobInfo(C_JobInfo structure) : base(structure)
        {
        }
    }
}
