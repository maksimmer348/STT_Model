using System;
using System.Diagnostics;
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
        /// Задается значение в секундах для проверки значений при запуске стенда
        /// </summary>
        public int Delay { get; private set; }

        /// <summary>
        /// Компорт прибора
        /// </summary>
        protected SerialPortInput port;

        /// <summary>
        /// Количество пингов если прибор выдал ошибку
        /// </summary>
        public int PingCountIfError { get; private set; }

        /// <summary>
        /// Ожидлаемое начало строки если прибор выдал ошибку
        /// </summary>
        public string? StartOfStringIfError { get; set; }

        /// <summary>
        /// Ожидлаемое окончание строки если прибор выдал ошибку
        /// </summary>
        public string EndOfStringIfError { get; private set; }

        /// <summary>
        /// Получение номера порта
        /// </summary>
        public string GetPortNum { get; private set; }

        /// <summary>
        /// DTR прибора
        /// </summary>
        private bool dtr;

        /// <summary>
        /// Класс библиотеки
        /// </summary>
        protected BaseLibCmd libCmd = new();

        public Action<BaseMeter, byte[]> Receive;
        public Action<BaseMeter, bool> ConnectPort;
        public Action<BaseMeter, bool> ConnectDevice;

        /// <summary>
        /// Конфигурация компортра утройства
        /// </summary>
        /// <param name="pornName">Номер компорта</param>
        /// <param name="baud">Бауд рейт компорта</param>
        /// <param name="stopBits">Стоповые биты компорта</param>
        /// <param name="parity">Parity bits</param>
        /// <param name="dataBits">Data bits count</param>
        /// <param name="dtr"></param>
        /// <returns></returns>
        public void Config(string pornName, int baud, StopBits stopBits, Parity parity, DataBits dataBits,
            bool dtr = false)
        {
            port = new SerialPortInput(new NullLogger<SerialPortInput>());
            port.SetPort(pornName, baud, stopBits, parity, dataBits);
            this.dtr = dtr;
            GetPortNum = pornName;
            port.ConnectionStatusChanged += OnPortConnectionStatusChanged;
            port.MessageReceived += OnPortMessageReceived;
        }

        public void PortConnect()
        {
            port.Connect();
        }

        public void PortDisconnect()
        {
            port.Disconnect();
        }

        //проверка порта
        void OnPortConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs args)
        {
            bool conn = args.Connected;
            ConnectPort.Invoke(this, conn);
            port.DtrEnable = dtr;
            //TODO для наглядности, потом убрать
            Console.WriteLine($"ответ от порта {GetPortNum} - {conn}, устройство {Name}");
            //
        }

        string str;
        //ответ от прибора

        private byte[] trashBytes = new byte[] {254, 252, 248, 255, 240};

        string ClearReceive(byte[] receive)
        {
            List<byte> InList = receive.ToList();
            foreach (var trashByte in trashBytes)
            {
                InList.Remove(trashByte);
            }

            return System.Text.Encoding.UTF8.GetString(InList.ToArray());
        }
        
        string GetBuffersString(string str)
        {
            // EndOfStringIfError ="";
            //StartOfStringIfError= "";
             if (string.IsNullOrEmpty(StartOfStringIfError) && string.IsNullOrEmpty(EndOfStringIfError) && PingCountIfError <= 0) return str;
             else if()
             {
                 
             }
             else if()
             {
                 
             }
        }


        void OnPortMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            //var data = System.Text.Encoding.UTF8.GetString(args.Data);
            var data = ClearReceive(args.Data);
            
          
            var answer = (data);
            //TODO для наглядности, потом убрать
            Console.WriteLine($"ответ от устройства {Name} - {answer}, порт {GetPortNum} ");
            //

            var selectCmd = libCmd.DeviceCommands
                .FirstOrDefault(x => x.Key.NameCmd == "Status" && x.Key.NameDevice == Name);
            if (answer.Contains(selectCmd.Value.Receive))
            {
                ConnectDevice.Invoke(this, true);
            }
        }

        /// <summary>
        /// Проверка устройства на коннект
        /// </summary>
        /// <param name="checkCmd">Команда проверки не из библиотеки (если пусто будет исользована команда "Status" и прибор из библиотеки )</param>
        /// <param name="delay">Задержка на проверку (если 0 будет исользована из библиотеки )</param>
        /// <returns>Успешна ли попытка коннекта</returns>
        /// <exception cref="DeviceException">Такого устройства, нет в библиотеке команд</exception>
        public void CheckedConnectDevice(string checkCmd = "", int delay = 0)
        {
            //Количество попыток досутчатся до прибора
            //TODO если достучались с первого раза то второй ненужно

            if (string.IsNullOrWhiteSpace(checkCmd))
            {
                TransmitCmdInLib("Status");
            }
            else
            {
                TransmitCmd(checkCmd);
            }
        }

        /// <summary>
        /// Отправка в устройство и прием СТАНДАРТНЫХ (есть в библиотеке команд) команд из устройства
        /// </summary>
        /// <param name="nameCommand">Имя команды (например Status)</param>
        public void TransmitCmdInLib(string nameCommand)
        {
            var selectCmd = libCmd.DeviceCommands
                .FirstOrDefault(x => x.Key.NameCmd == nameCommand && x.Key.NameDevice == Name).Value;
            Delay = selectCmd.Delay;
            PingCountIfError = selectCmd.PingCountIfError;
            EndOfStringIfError = selectCmd.EndOfStringIfError;
            if (selectCmd == null)
            {
                throw new DeviceException(
                    $"Такой команды - {nameCommand} или такого утройства {Name}, нет в библиотеке команд");
            }

            TransmitCmd(selectCmd.Transmit, Delay, selectCmd.Terminator);
        }

        /// <summary>
        /// Отправка в устройство и прием команд из устройства
        /// </summary>
        /// <param name="cmd">Команда</param>
        /// <param name="delay">Задержка между запросом и ответом</param>
        /// <param name="receiveType"></param>
        /// <param name="terminator">Окончание строки команды по умолчанию \n</param>
        /// <returns>Ответ от устройства</returns>
        public void TransmitCmd(string cmd, int delay = 0, string terminator = "\n")
        {
            Delay = delay;
            var message = System.Text.Encoding.UTF8.GetBytes(cmd + terminator);
            port.SendMessage(message);
        }
    }
}