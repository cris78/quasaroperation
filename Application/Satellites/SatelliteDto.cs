using System.Collections.Generic;

namespace Application.Satellites
{
    public class SatelliteDto
    {
        public string Name { get; set; }
        public double Distance { get; set; }
        public List<string> Message { get; set; }
    }
}