namespace Application.Satellites
{
    public class ShipResponse
    {
        public Position Position { get; set; }
        public string Message { get; set; }
    }

    public class Position
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}