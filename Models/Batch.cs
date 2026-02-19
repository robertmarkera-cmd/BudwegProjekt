using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Budweg.Models
{
    public class Batch
    {
        public string Comment { get; set; }
        public int BatchNumber { get; set; }
        public int BatchAmount { get; set; }
        public Bitmap Picture { get; set; }
    }
}