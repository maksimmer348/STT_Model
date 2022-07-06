namespace Vips
{


    public class TypeVip
    {
        public string Type { get; set; }

        //максимальные значения во время предпотготовки испытания 
        public int PrepareMaxCurrent { get; set; }
        public int PrepareMaxVoltage1 { get; set; }
        public int PrepareMaxVoltage2 { get; set; }
        
        //максимаьные значения во время испытаниий они означают ошибку
        public int MaxTemperature { get; set; }
        public int MaxCurrent { get; set; }
        public int MaxVoltage1 { get; set; }
        public int MaxVoltage2 { get; set; }

    }
}