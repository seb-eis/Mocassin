using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Mocassin.Model.Translator
{
    /// <summary>
    /// Occupation code list alias class. Represents a linear list of occupation lookup codes for the simulation
    /// </summary>
    public class OccupationCodeListEntity : InteropBinaryList<long>
    {

    }
}
