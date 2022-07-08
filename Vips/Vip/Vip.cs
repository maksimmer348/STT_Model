namespace Vips
{
    public class Vip
    {
        public string Name { get; set; }
        public TypeVip Type { get; set; }

        public double Temperature { get; set; }
        public double VoltageIn { get; set; }
        public double VoltageOut1 { get; set; }
        public double VoltageOut2 { get; set; }
        public double CurrentIn { get; set; }
        public StatusVip Status { get; set; }
    }
}