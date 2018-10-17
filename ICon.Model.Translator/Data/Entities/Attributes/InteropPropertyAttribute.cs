using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mocassin.Model.Translator
{
    public class InteropPropertyAttribute : Attribute
    {
        public string BinaryPropertyName { get; }

        public InteropPropertyAttribute(string binaryPropertyName)
        {
            BinaryPropertyName = binaryPropertyName ?? throw new ArgumentNullException(nameof(binaryPropertyName));
        }
    }
}
