using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICon.Model.Translator
{
    public class TestEntity : EntityBase
    {
        [ForeignKey(nameof(Matrix))]
        public int MatrixId { get; set; }

        public MatrixEntity<int> Matrix { get; set; }
    }
}
