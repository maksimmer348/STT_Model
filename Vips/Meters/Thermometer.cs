namespace Vips

{
    public class Thermometer : BaseDevice
    {
        public double Temperature { get; set; }

        public Thermometer(string name, TypeDevice type) : base(name, type)
        {
        }
    }
}