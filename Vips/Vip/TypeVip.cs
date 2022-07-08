namespace Vips
{
    public class TypeVip
    {
        public string Type { get; set; }


        //максимаьные значения во время испытаниий они означают ошибку
        public double MaxTemperature { get; set; }
        public double MaxVoltageIn { get; set; }

        private double maxVoltageOut1;

        public double MaxVoltageOut1
        {
            get => maxVoltageOut1;
            set
            {
                maxVoltageOut1 = value;
                PrepareMaxVoltageOut1 = value;
            }
        }

        private double maxVoltageOut2;

        public double MaxVoltageOut2
        {
            get => maxVoltageOut2;
            set
            {
                maxVoltageOut2 = value;
                PrepareMaxVoltageOut2 = value;
            }
        }

        public double MaxCurrentIn { get; set; }

        //максимальные значения во время предпотготовки испытания 
        public double PrepareMaxCurrentIn { get; set; }
        public double PrepareMaxVoltageOut1 { get; set; }
        public double PrepareMaxVoltageOut2 { get; set; }
    }
}