using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Budweg.Models
{
    public class Caliper
    {
        public string Type { get; set; }
        public string  Manufacturer { get; set; }
        public string Comment { get; set; }
        public Bitmap Picture { get; set; }
        public int FrameID { get; set; }

    }
}
