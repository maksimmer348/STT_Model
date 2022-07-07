using System.Globalization;

namespace Vips
{
    public class BaseSerial
    {
        public int PortNum { get; set; }
        public int BaudRate { get; }
        public int StopBits { get; }

        private SerialPort serial = new SerialPort();

        public BaseSerial(int portNum, int baudRate, int stopBits)
        {
            PortNum = portNum;
            BaudRate = baudRate;
            StopBits = stopBits;
        }

        public void WriteString(string write)
        {
            try
            {
                
                //gsp.WriteAsciiString(write + "\r\n");
                serial.Write(write);
            }
            catch (DeviceException e)
            {
                throw new DeviceException($"Попытка записать данные была неуспешной ошибка - {e}");
            }
        }

        public string ReadString()
        {
            try
            {
                return serial.Read();
            }
            catch (DeviceException e)
            {
                throw new DeviceException($"Попытка прочитаь данные была неуспешной ошибка - {e}");
            }
        }



        //TODO продолжить список
        //TODO уточнить название 
        private string[] invalidSymbols = new[] {"\n", "\r"};

        protected double ReadDouble()
        {
            var receive = ReadString();
            foreach (var t in invalidSymbols)
            {
                receive = receive.Replace(t, "");
            }
            if (!double.TryParse(receive, NumberStyles.Any, CultureInfo.InvariantCulture,out double i))
            {
                throw new DeviceException($"Значние {receive} не удалось привести к числу");
            }
            return i;
        }
    }
}