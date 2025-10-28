using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplicationzhh
{
    public class Curves
    {
        public List<曲线类> pCurves = new List<曲线类>();

        public Curves Copy()
        {
            var copy = new Curves();
            foreach (var curve in pCurves)
            {
                copy.pCurves.Add(curve.Clone());
            }
            return copy;

        }

    }
}
