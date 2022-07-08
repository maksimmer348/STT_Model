using System;

namespace Vips
{
    public class BaseMeter
    {
        public string Name { get; set; }

        /// <summary>
        /// Задается значение в секундах для проверки значений при запуске стенда
        /// </summary>
        public int DelayBetween { get; set; }

        protected TypeDevice type;
        protected BaseSerial port;
        protected BaseLibCmd libCmd = new();

        /// <summary>
        /// Конфигурация компортра утройства
        /// </summary>
        /// <param name="portNum">Номер компорта</param>
        /// <param name="baudRate">Бауд рейт компорта</param>
        /// <param name="stopBits">Стоповые биты компорта</param>
        /// <param name="check">Нужна ли проверка на коннект от утсройства</param>
        /// <param name="checkedOnConnectTimes">Количество запросов на устройство в случае если не удалось получить ответ</param>
        /// <returns></returns>
        public virtual bool Config(int portNum, int baudRate, int stopBits, int checkedOnConnectTimes = 1)
        {
            port = new BaseSerial(portNum, baudRate, stopBits);

            if (checkedOnConnectTimes >= 1)
            {
                return CheckedConnect(checkedOnConnectTimes);
            }

            return true;
        }

        /// <summary>
        /// Проверка устройства на коннект
        /// </summary>
        /// <param name="checkedOnConnectTimes">Количество попыток подключится к устройству</param>
        /// <returns>Успешна ли попытка коннекта</returns>
        /// <exception cref="DeviceException">Такого устройства, нет в библиотеке команд</exception>
        public virtual bool CheckedConnect(int checkedOnConnectTimes = 1)
        {
            var selectCmd = libCmd.DeviceCommands
                .FirstOrDefault(x => x.Key.NameCmd == "Status" && x.Key.NameDevice == Name);

            if (selectCmd.Value == null)
            {
                //TODO предложить добавить по этому иключению новое устройство
                throw new DeviceException(
                    $"Такого утройства {Name}, нет в библиотеке команд");
            }

            //Количество попыток досутчатся до прибора
            for (int i = 0; i < checkedOnConnectTimes; i++)
            {
                //TODO задать задержку между командаой и ответом
                //TODO заглушка вместо задержки 
                int tempDelay = selectCmd.Value.Delay;
                Console.WriteLine($"Задержка \"Checked\" {tempDelay} мс == \"TransmitReceivedCmd\"");

                string receive = TransmitReceivedCmd(selectCmd.Value.Transmit, tempDelay);
                //TODO разобратся шаблон ответа должен в сбее содержать ответ прибора или наоборот
                if (receive.Contains(selectCmd.Value.Receive))
                {
                    Console.WriteLine($"Устройство {Name}, успешно прошло проверку");
                    //Уведомить
                    return true;
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
        public string TransmitReceivedDefaultCmd(string nameCommand, int delay = 0)
        {
            var selectCmd = libCmd.DeviceCommands
                .FirstOrDefault(x => x.Key.NameCmd == nameCommand && x.Key.NameDevice == Name);

            if (selectCmd.Value == null)
            {
                throw new DeviceException(
                    $"Такой команды - {nameCommand} или такого утройства {Name}, нет в библиотеке команд");
            }

            //TODO заглушка вместо задержки 
            int tempDelay = delay;
            Console.WriteLine($"Задержка \"TransmitReceivedDefaultCmd\" {tempDelay} мс == \"TransmitReceivedCmd\"");

            //Если в метод не передается иное значение задержки то используется стандартная из библиотеки команд
            if (delay == 0)
            {
                //TODO задать задержку между командаой и ответом
                //TODO заглушка вместо задержки 
                tempDelay = selectCmd.Value.Delay;
                Console.WriteLine($"Задержка \"TransmitReceivedDefaultCmd\" {tempDelay} мс == \"TransmitReceivedCmd\"");
            }
            
            return TransmitReceivedCmd(selectCmd.Value.Transmit, tempDelay);
        }

        /// <summary>
        /// Отправка в устройство и прием команд из устройства
        /// </summary>
        /// <param name="cmd">Команда</param>
        /// <param name="delay">Задержка между запросом и ответом</param>
        /// <param name="receiveType"></param>
        /// <returns>Ответ от устройства</returns>
        public string TransmitReceivedCmd(string cmd, int delay)
        {
            port.WriteString(cmd);

            //TODO заглушка вместо задержки 
            int tempDelay = delay;

            DelayBetween = delay;
            Console.WriteLine($"Задержка \"TransmitReceivedCmd\" {tempDelay} мс");
            
            return  port.ReadString();
        }
        
    }
}