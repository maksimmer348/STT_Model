namespace Vips
{

    /// <summary>
    /// Стандартная команда, ответ и задержка для проверки устройства на коннект
    /// </summary>
    public class MeterCmd
    {
        public string ToPortCmd { get; set; }
        public string InPorToCmd { get; set; }
        public int Delay { get; set; }
    }
}