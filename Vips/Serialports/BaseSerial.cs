namespace Vips
{
    public class BaseSerial
    {
        public int PortNum { get; set; }
        public int BaudRate { get; }
        public int StopBits { get; }

        private SerialPort serial;

        public BaseSerial(int portNum, int baudRate, int stopBits)
        {
            PortNum = portNum;
            BaudRate = baudRate;
            StopBits = stopBits;
        }

        public void Write(string write)
        {
            serial.Write(write);
            //write -> to port
        }

        public string Read()
        {
            try
            {
               return serial.Read();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Попытка прочитаь данные была неуспешной ошибка - {e}");
                throw;
            }
        }
    }
}