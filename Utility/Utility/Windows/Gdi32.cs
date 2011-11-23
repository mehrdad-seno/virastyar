using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace SCICT.Utility.Windows
{
    public class Gdi32
    {
        [DllImport("gdi32.dll")]
        public static extern IntPtr GetStockObject(int fnObject);
    }
}
