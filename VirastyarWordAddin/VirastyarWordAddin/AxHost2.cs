using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using stdole;

namespace VirastyarWordAddin
{
    public class AxHost2 : AxHost
    {
        public AxHost2()
            : base("59EE46BA-677D-4d20-BF10-8D8067CB8B33")
        {
        }

        public new static IPictureDisp GettIPictureDispFromPicture(Image image)
        {
            return (IPictureDisp)AxHost.GetIPictureDispFromPicture(image);
        }
    }
}
