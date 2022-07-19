using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using RJCP.IO.Ports;
using SerialPortLib;
using System.Threading;


namespace Vips
{
    public class BaseMeter
    {
        /// <summary>
        /// Имя прибора
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип прибора
        /// </summary>
        public TypeDevice Type;

        /// <summary>
        /// Компорт прибора
        /// </summary>
        protected ISerialLib port;

        /// <summary>
        /// Класс библиотеки
        /// </summary>
        protected BaseLibCmd libCmd = new();

        /// <summary>
        /// Событие приема данных с устройства
        /// </summary>
        public Action<BaseMeter, byte[]> Receive;

        /// <summary>
        /// Событие проверки коннекта к порту
        /// </summary>
        public Action<BaseMeter, bool> ConnectPort;

        Stopwatch stopwatch = new();

        /// <summary>
        /// Конфигурация компортра утройства
        /// </summary>
        /// <param name="typePort">Тип исопльзуемой библиотеки com port</param>
        /// <param name="pornName">Номер компорта</param>
        /// <param name="baud">Бауд рейт компорта</param>
        /// <param name="stopBits">Стоповые биты компорта</param>
        /// <param name="parity">Parity bits</param>
        /// <param name="dataBits">Data bits count</param>
        /// <param name="dtr"></param>
        /// <returns></returns>
        public void ConfigDevice(TypePort typePort, string pornName, int baud, int stopBits, int parity, int dataBits,
            bool dtr = false)
        {
            if (typePort == TypePort.GodSerial)
            {
                port = new SerialGod();
            }
            else if (typePort == TypePort.SerialInput)
            {
                port = new SerialInput();
            }

            port.SetPort(pornName, baud, stopBits, parity, dataBits);
            port.Dtr = dtr;
            port.ConnectionStatusChanged += ConnectionStatusChanged;
            port.MessageReceived += MessageReceived;
        }

        public void PortConnect()
        {
            port.Connect();
        }

        public void PortDisconnect()
        {
            port.Disconnect();
        }

        /// <summary>
        /// Проверка устройства на коннект
        /// </summary>
        /// <param name="checkCmd">Команда проверки не из библиотеки (если пусто будет исользована команда "Status" и прибор из библиотеки )</param>
        /// <param name="delay">Задержка на проверку (если 0 будет исользована из библиотеки )</param>
        /// <returns>Успешна ли попытка коннекта</returns>
        /// <exception cref="DeviceException">Такого устройства, нет в библиотеке команд</exception>
        public void CheckedConnectDevice(string checkCmd = "", int delay = 0, string terminator = "")
        {
            //для отладки
            // Время начала 
            stopwatch.Start();
            //

            //если строка команды пустая
            if (string.IsNullOrWhiteSpace(checkCmd))
            {
                //используем команду статус которя возмет текущий прибор и введет в него команду статус
                TransmitCmdInLib("Status");
            }
            else
            {
                //используем ручной ввод
                port.TransmitCmd(cmd: checkCmd, delay: delay, terminator: terminator);
            }
        }

        /// <summary>
        /// Отправка в устройство и прием СТАНДАРТНЫХ (есть в библиотеке команд) команд из устройства
        /// </summary>
        /// <param name="nameCommand">Имя команды (например Status)</param>
        public void TransmitCmdInLib(string nameCommand)
        {
            // MeterCmd selectCmd = libCmd.DeviceCommands
            //     .FirstOrDefault(x => x.Key.NameCmd == nameCommand && x.Key.NameDevice == Name).Value;
            var selectCmd = GetLibItem(nameCommand, Name);

            if (selectCmd == null)
            {
                throw new DeviceException(
                    $"Такое устройство - {Name} или команда - {nameCommand} в библиотеке не найдены");
            }

            if (selectCmd.MessageType == TypeCmd.Hex)
            {
                port.TransmitCmd(GetStringHexInText(selectCmd.Transmit), selectCmd.Delay,
                    GetStringHexInText(selectCmd.StartOfString), GetStringHexInText(selectCmd.EndOfString),
                    GetStringHexInText(selectCmd.Terminator));
            }
            else
            {
                port.TransmitCmd(selectCmd.Transmit, selectCmd.Delay, selectCmd.StartOfString, selectCmd.EndOfString,
                    selectCmd.Terminator);
            }
        }

        /// <summary>
        /// Обработка прнятого сообщения из устройства
        /// </summary>
        private void MessageReceived(string receive)
        {
            //для отладки
            Console.WriteLine("Время работы программы: {0} милисекунд", stopwatch.Elapsed.TotalMilliseconds);
            stopwatch.Restart(); // Остановить отсчет времени

            //для отладки
            Console.WriteLine($"ответ от устройства {Name} - {receive}, порт {port.GetPortNum} ");
            //
            //для проверки на статус 
            var selectCmd = GetLibItem("Status", Name);
            //если ответ от устройства соотвествует ответу на кодмаду Status то вернем true
            if (receive.Contains(selectCmd.Receive))
            {
                ConnectPort.Invoke(this, true);
            }
        }

        /// <summary>
        /// Прошел ли коннект выбраного com port
        /// </summary>
        private void ConnectionStatusChanged(bool obj)
        {
            //для отладки
            Console.WriteLine($"ответ от порта {port.GetPortNum} - {obj}, устройство {Name}");
            //
        }

        protected MeterCmd GetLibItem(string cmd, string deviceName)
        {
            return libCmd.DeviceCommands
                .FirstOrDefault(x => x.Key.NameCmd == cmd && x.Key.NameDevice == deviceName).Value;
        }

        string GetStringTextInHex(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                byte[] bytes = new byte[s.Length / 2];
                for (int i = 0; i < s.Length; i += 2)
                {
                    var ff = bytes[i / 2];
                    bytes[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
                }

                return Encoding.ASCII.GetString(bytes);
            }

            return "";
        }

        string GetStringHexInText(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                string hex = "";
                foreach (var ss in s)
                {
                    hex += Convert.ToByte(ss).ToString("x2");
                }

                return hex;
            }

            return "";
        }
    }
}