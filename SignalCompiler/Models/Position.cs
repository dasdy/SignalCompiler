using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalCompiler.Models
{
    public class Position
    {
        public int Line { get; set; }
        public int Column { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}",Line,Column);
        }
    }
}
