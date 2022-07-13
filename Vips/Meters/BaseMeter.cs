using System;
using Microsoft.Extensions.Logging.Abstractions;
using RJCP.IO.Ports;
using SerialPortLib;
using System.Threading;


namespace Vips
{
    public class BaseMeter
    {
        public string Name { get; set; }

        /// <summary>
        /// Задается значение в секундах для проверки значений при запуске стенда
        /// </summary>
        public string GetPortNum { get; set; }

        public TypeDevice Type;
        protected BaseLibCmd libCmd = new();

        protected SerialPortInput port;
        public Action<BaseMeter,byte[]> Receive;
        public Action<BaseMeter, bool> ConnectPort;
        public Action<BaseMeter, bool> ConnectDevice;

        /// <summary>
        /// Конфигурация компортра утройства
        /// </summary>
        /// <param name="portNum">Номер компорта</param>
        /// <param name="baudRate">Бауд рейт компорта</param>
        /// <param name="stopBits">Стоповые биты компорта</param>
        /// <param name="check">Нужна ли проверка на коннект от утсройства</param>
        /// <param name="checkedOnConnectTimes">Количество запросов на устройство в случае если не удалось получить ответ</param>
        /// <returns></returns>
        public void Config(string pornName, int baud, StopBits stopBits, Parity parity, DataBits dataBits)
        {
            port = new SerialPortInput(new NullLogger<SerialPortInput>());
            port.SetPort(pornName, baud, stopBits, parity, dataBits);
            GetPortNum = pornName;
            port.ConnectionStatusChanged += OnPortOnConnectionStatusChanged;
            port.MessageReceived += OnPortOnMessageReceived;
        }

        public bool PortConnect()
        {
            return port.Connect();
        }

        public void PortDisconnect()
        {
            port.Disconnect();
        }

        void OnPortOnConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs args)
        {
            ConnectPort.Invoke(this, args.Connected);
        }
        
        void OnPortOnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            var data = System.Text.Encoding.UTF8.GetString(args.Data);
           var selectCmd = libCmd.DeviceCommands
               .FirstOrDefault(x => x.Key.NameCmd == "Status" && x.Key.NameDevice == Name);
            if (data.Contains(selectCmd.Value.Receive))
            {
                ConnectDevice.Invoke(this, true);
            }
        }


        /// <summary>
        /// Проверка устройства на коннект
        /// </summary>
        /// <param name="checkedOnConnectTimes">Количество попыток подключится к устройству</param>
        /// <param name="checkCmd">Команда проверки не из библиотеки (если пусто будет исользована команда из библиотеки "Status")</param>
        /// <returns>Успешна ли попытка коннекта</returns>
        /// <exception cref="DeviceException">Такого устройства, нет в библиотеке команд</exception>
        public bool CheckedConnect(int checkedOnConnectTimes = 1, string checkCmd = "")
        {
            var selectCmd = libCmd.DeviceCommands
                .FirstOrDefault(x => x.Key.NameCmd == "Status" && x.Key.NameDevice == Name);

            if (selectCmd.Value == null)
            {
                //TODO M предложить добавить по этому иключению новое устройство
                throw new DeviceException(
                    $"Такого утройства {Name}, нет в библиотеке команд");
            }

            //Количество попыток досутчатся до прибора
            //TODO если достучались с первого раза то второй ненужно
            for (int i = 0; i < checkedOnConnectTimes; i++)
            {
                if (string.IsNullOrWhiteSpace(checkCmd))
                {
                    TransmitReceivedCmd(selectCmd.Value.Transmit);
                }
                else
                {
                    TransmitReceivedCmd(checkCmd);
                }
            }

            Console.WriteLine($"Устройство {Name}, не смогло пройти проверку");
            //Уведомить
            return false;
        }

        /// <summary>
        /// Отправка в устройство и прием СТАНДАРТНЫХ (есть в библиотеке команд) команд из устройства
        /// </summary>
        /// <param name="nameCommand">Имя команды (например Status)</param>
        /// <param name="nameDevice">Имя девайса (например GPS-74303)</param>
        /// <param name="delay">Задержка между запросом и ответом если 0, то используется стандартная из библиотеки команд</param>
        /// <param name="templateCommand">Будет ли использоватся стандартный ответ от прибора например GWInst</param>
        /// <returns>Ответ от устройства</returns>
        public void TransmitReceivedDefaultCmd(string nameCommand, int delay = 0)
        {
            var selectCmd = libCmd.DeviceCommands
                .FirstOrDefault(x => x.Key.NameCmd == nameCommand && x.Key.NameDevice == Name);

            if (selectCmd.Value == null)
            {
                throw new DeviceException(
                    $"Такой команды - {nameCommand} или такого утройства {Name}, нет в библиотеке команд");
            }

            //Если в метод не передается иное значение задержки то используется стандартная из библиотеки команд
            if (delay == 0)
            {
                delay = selectCmd.Value.Delay;
            }

            TransmitReceivedCmd(selectCmd.Value.Transmit);
        }

        /// <summary>
        /// Отправка в устройство и прием команд из устройства
        /// </summary>
        /// <param name="cmd">Команда</param>
        /// <param name="delay">Задержка между запросом и ответом</param>
        /// <param name="receiveType"></param>
        /// <returns>Ответ от устройства</returns>
        public void TransmitReceivedCmd(string cmd)
        {
            var message = System.Text.Encoding.UTF8.GetBytes(cmd + "\n");
            port.SendMessage(message);
            
            //Thread.Sleep(delay);
            // Console.WriteLine($"Задержка \"TransmitReceivedCmd\" {delay} мс");
        }
    }
}