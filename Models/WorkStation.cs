using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Budweg.Models
{
    public class WorkStation
    {
        public int OperationalNumber { get; set; }
        public DateTime Arival { get; set; }
        public DateTime Departure { get; set; }
        public int ArrivalNumber { get; set; }
        public int DepartureNumber { get; set; }
        public int WasteNumber { get; set; }
        public string WorkStationName { get; set; }
    }
}
