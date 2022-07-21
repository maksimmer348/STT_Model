namespace Vips
{
    public class VoltMeter : BaseDevice
    {
        public double Volt { get; set; }
        public double Current { get; set; }

        public VoltMeter(string name, TypeDevice type) : base(name, type)
        {
        }
    }
}