using System;
using System.Collections.Generic;
using System.Text;

namespace Budweg.Models
{
    public class Caliper
    {
        public string Type { get; set; }
        public string  Manufacturer { get; set; }
        public string Comment { get; set; }
        // store image path (string) instead of Bitmap to simplify persistence
        public string Picture { get; set; }
        public int FrameID { get; set; }

    }
}
