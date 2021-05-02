using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class Satellite
    {
        public Satellite()
        {

        }

        public Satellite(Guid id, string name, double distance, double x, double y)
        {
            this.Id = id;
            this.Name = name;
            this.Distance = distance;
            this.X = x;
            this.Y = y;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Distance { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        [NotMapped]
        public List<string> Message { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}