using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Triple<T, U, V, W>
    {
        public T Item1 { get; set; }
        public U Item2 { get; set; }
        public V Item3 { get; set; }
        public W Item4 { get; set; }

        public Triple(T i1, U i2, V i3, W i4)
        {
            Item1 = i1;
            Item2 = i2;
            Item3 = i3;
            Item4 = i4;
        }
    }
}
