﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Translator
{
    public class EnergyBackgroundEntity : InteropBinaryArray<double>
    {
        public override string BlobTypeName => "LEB";
    }
}
