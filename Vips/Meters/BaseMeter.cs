using System;

namespace Vips
{
    public class BaseMeter
    {
        public string Name { get; set; }
        private TypeDevice type;
        private BaseSerial port;
        private BaseLibCmd libCmd = new();
        
        public bool Config(int portNum, int baudRate, int stopBits, int checkTimes = 1, bool check = true)
        {
            port = new BaseSerial(portNum, baudRate, stopBits);
            
            if (check)
            {
                return Checked(checkTimes);
            }
            return true;
        }

        public bool Checked(int checkTimes = 1)
        {
            //TODO если значение из компорта рано ожидаемому значению из библиотеки команд то тру
            //return TransmitReceivedCmd("Status", Name) == ожидаемому из 
            Console.WriteLine(Name + " не работает");
            return false;
        }

        /// <summary>
        /// Отправка команды в устройство и прием команды из устройства
        /// </summary>
        /// <param name="nameCommand">Имя команды например Status</param>
        /// <param name="nameDevice">Имя девайса например GPS-74303</param>
        /// <param name="delay">Задержка между запросом и ответом</param>
        /// <param name="templateCommand">Будет ли использоватся стандартный ответ от прибора например GWInst</param>
        /// <returns></returns>
        public string TransmitReceivedCmd(string nameCommand, string nameDevice, int delay = 50)
        {
            var selectCmd = libCmd.DeviceCommands
                .Where(x => x.Key.NameCmd == nameCommand)
                .FirstOrDefault(x => x.Key.NameDevice == nameDevice);

            port.Write(selectCmd.Value.Transmit);
            //int delay = selectCmd.Delay;
            string receive = port.Read();
            return receive;
        }
    }
}