using System.Collections.Generic;

namespace Vips
{
//TODO сделать инстантом -> OK
//TODO добавиьт сериализацию команд потом (не обязательно)
    public class BaseLibCmd
    {
        public Dictionary<MeterIdentCmd, MeterCmd> DeviceCommands { get; set; } =
            new Dictionary<MeterIdentCmd, MeterCmd>();

        public BaseLibCmd()
        {
            DeviceCommands.Add(
                new MeterIdentCmd()
                {
                    NameDevice = "GPS-74303",
                    NameCmd = "Status"
                },
                new MeterCmd()
                {
                    //запрос
                    Transmit = "IDN?",
                    //ожидаемый ответ
                    Receive = "Ok",
                    //задержка между запросом и ответом 
                    Delay = 50
                });
            
            DeviceCommands.Add(
                new MeterIdentCmd()
                {
                    NameDevice = "GPS-74303",
                    NameCmd = "Voltage?"
                },
                new MeterCmd()
                {
                    //запрос
                    Transmit = "Volt?",
                    //ожидаемый ответ
                });

            DeviceCommands.Add(new MeterIdentCmd()
            {
                NameDevice = "GPS-90",
                NameCmd = "Status"
            }, new MeterCmd() {Transmit = "Status?", Receive = "GWInst", Delay = 50});

            DeviceCommands.Add(new MeterIdentCmd()
            {
                NameDevice = "DPO-3014",
                NameCmd = "Status"
            }, new MeterCmd() {Transmit = "HEH?", Receive = "Ok", Delay = 50});

            DeviceCommands.Add(new MeterIdentCmd()
            {
                NameDevice = "DPO-3014",
                NameCmd = "OutputOn",
            }, new MeterCmd() {Transmit = "cmdon", Delay = 50});


            DeviceCommands.Add(new MeterIdentCmd()
            {
                NameDevice = "DPO-3014",
                NameCmd = "OutputOff"
            }, new MeterCmd() {Transmit = "cmdoff", Delay = 50});

            DeviceCommands.Add(
                new MeterIdentCmd()
                {
                    NameDevice = "Relay",
                    NameCmd = "Status"
                },
                new MeterCmd()
                {
                    Transmit = "Status?",
                    Receive = "Ok",
                    Delay = 50
                });
            
        }


        /// <summary>
        /// Добавление команды в общую билиотеку команд
        /// </summary>
        /// <param name="nameCommand">Имя команды</param>
        /// <param name="nameDevice">Прибор для которого эта команда предназначена</param>
        /// <param name="transmitCmd">Команда котороую нужно передать в прибор</param>
        /// <param name="receiveCmd">Ответ от прибора на команду</param>
        /// <param name="delayCmd">Задержка между передачей команды и приемом ответа</param>
        public void AddCommand(string nameCommand, string nameDevice, string transmitCmd, string receiveCmd,
            int delayCmd)
        {
            var tempIdentCmd = new MeterIdentCmd {NameCmd = nameCommand, NameDevice = nameDevice};
            var tempCmd = new MeterCmd() {Transmit = transmitCmd, Receive = receiveCmd, Delay = delayCmd};
            DeviceCommands.Add(tempIdentCmd, tempCmd);

            Console.WriteLine(
                $"была добавлена команда {tempIdentCmd.NameCmd} для прибора{tempIdentCmd.NameDevice}");
            //уведомить
        }

        /// <summary>
        /// Удаление команды устройства
        /// </summary>
        /// <param name="nameCommand">Имя удаляемой команды</param>
        /// <param name="nameDevice">Имя устройства команду которого удаляют</param>
        public void DeleteCommand(string nameCommand, string nameDevice)
        {
            var select = DeviceCommands
                .Where(x => x.Key.NameCmd == nameCommand)
                .FirstOrDefault(x => x.Key.NameDevice == nameDevice).Key;

            if (DeviceCommands.ContainsKey(select))
            {
                Console.WriteLine(
                    $"была удалена команда {select.NameCmd} для прибора тип {select.NameDevice}, имя прибора {select.NameDevice}");
                //уведомить

                DeviceCommands.Remove(select);
            }
        }
    }
}