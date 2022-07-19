﻿using System.Collections.Generic;

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
            #region Statuses

            //команда с шаблоном ответа
            DeviceCommands.Add(
                new MeterIdentCmd()
                {
                    //имя устройктсва
                    NameDevice = "GDM-78255A",
                    //имя команды
                    NameCmd = "Status"
                },
                new MeterCmd()
                {
                    //запрос
                    Transmit = "*IDN?",
                    //окончание строки
                    Terminator = "\n",
                    //ожидаемый ответ
                    Receive = "78255",
                    //тип ожидаемого ответа - текстовый
                    MessageType = TypeCmd.Text,
                    //задержка между запросом и ответом 
                    Delay = 50
                });

              //команда с шаблоном ответа
            DeviceCommands.Add(
                new MeterIdentCmd()
                {
                    //имя устройктсва
                    NameDevice = "PSW7-800-2.88",
                    //имя команды
                    NameCmd = "Status"
                },
                new MeterCmd()
                {
                    //запрос
                    Transmit = "*IDN?",
                    //окончание строки
                    Terminator = "\n",
                    //ожидаемый ответ
                    Receive = "800-2.88",
                    //задержка между запросом и ответом 
                    Delay = 50
                });
           
                
            DeviceCommands.Add(
                new MeterIdentCmd()
                {
                    //имя устройктсва
                    NameDevice = "PSP-405",
                    //имя команды
                    NameCmd = "Status"
                },
                new MeterCmd()
                {
                    //запрос
                    Transmit = "W",
                    //окончание строки
                    Terminator = "\r\n",
                    //ожидаемый ответ
                    Receive = "00.0",
                    //тип ожидаемого ответа - текстовый
                    MessageType = TypeCmd.Hex,
                    //задержка между запросом и ответом 
                    Delay = 50,

                    StartOfString = "W",
                    PingCount = 3,
                    EndOfString = "\r\n",
                });

            #endregion

            #region RelayCommands

            DeviceCommands.Add(
                new MeterIdentCmd()
                {
                    NameDevice = "Реле-1",
                    NameCmd = "On"
                },
                new MeterCmd()
                {
                    Transmit = "4e50617f",
                    Receive = "Ok",
                    MessageType = TypeCmd.Hex,
                    Delay = 5000
                });

            DeviceCommands.Add(
                new MeterIdentCmd()
                {
                    NameDevice = "Реле-1",
                    NameCmd = "Off"
                },
                new MeterCmd()
                {
                    Transmit = "4e50b8A6",
                    Receive = "Ok",
                    MessageType = TypeCmd.Hex,
                    Delay = 5000
                });

            DeviceCommands.Add(
                new MeterIdentCmd()
                {
                    NameDevice = "Реле-2",
                    NameCmd = "On"
                },
                new MeterCmd()
                {
                    Transmit = "4e50617f",
                    Receive = "Ok",
                    MessageType = TypeCmd.Hex,
                    Delay = 5000
                });

            DeviceCommands.Add(
                new MeterIdentCmd()
                {
                    NameDevice = "Реле-2",
                    NameCmd = "Off"
                },
                new MeterCmd()
                {
                    Transmit = "4e50b8A6",
                    Receive = "Ok",
                    MessageType = TypeCmd.Hex,
                    Delay = 5000
                });

            #endregion
        }

        /// <summary>
        /// Добавление команды в общую билиотеку команд
        /// </summary>
        /// <param name="nameCommand">Имя команды</param>
        /// <param name="nameDevice">Прибор для которого эта команда предназначена</param>
        /// <param name="transmitCmd">Команда котороую нужно передать в прибор</param>
        /// <param name="receiveCmd">Ответ от прибора на команду</param>
        /// <param name="delayCmd">Задержка между передачей команды и приемом ответа</param>\
        /// <param name="type">Тип ответа (по умолчанию текстовый)</param>
        /// <param name="startOfString">Начало строки для библиотеки SerialGod</param>
        /// <param name="endOfString"> Конец строки для библиотеки SerialGod </param>
        /// <param name="pingCount">Количество попыток на считывание команды для библиотеки SerialGod - попытка += ~30мс </param>
        public void AddCommand(string nameCommand, string nameDevice, string transmitCmd, string receiveCmd,
            int delayCmd, string startOfString = "", string endOfString = "", int pingCount = 0,
            TypeCmd type = TypeCmd.Text)
        {
            var tempIdentCmd = new MeterIdentCmd
            {
                NameCmd = nameCommand,
                NameDevice = nameDevice
            };
            var tempCmd = new MeterCmd
            {
                Transmit = transmitCmd,
                Receive = receiveCmd,
                MessageType = type,
                Delay = delayCmd,
                StartOfString = startOfString,
                EndOfString = endOfString,
                PingCount = pingCount
            };

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

        /// <summary>
        ///  Изменить значение команды по ключу
        /// </summary>
        /// <param name="nameCommandOld">Название изменяемой команды</param>
        /// <param name="nameDeviceOld">Название прибора для кторого будет изменена команда</param>
        /// <param name="transmitCmdNew">Новое значение передваемой команды (если пусто исользуется старая команда)</param>
        /// <param name="receiveCmdNew">Новое значение принримаемой команды (если пусто исользуется старая команда)</param>
        /// <param name="delayCmdNew">Новое значение задержки (если 0 исользуется старая задержка)</param
        /// <param name="typeNew">Новое значение типа сообщения (если пусто исользуется старый тип)</param>
        /// <param name="startOfStringNew">Начало строки для библиотеки SerialGod</param>
        /// <param name="endOfStringNew"> Конец строки для библиотеки SerialGod </param>
        /// <param name="pingCountNew">Количество попыток на считывание команды для библиотеки SerialGod - попытка += ~30мс </param>
        public void ChangeCommand(string nameCommandOld, string nameDeviceOld, string transmitCmdNew = "",
            string receiveCmdNew = "", int delayCmdNew = 0, string startOfStringNew = "", string endOfStringNew = "",
            int pingCountNew = 0,
            TypeCmd typeNew = TypeCmd.Text)
        {
            var select = DeviceCommands
                .Where(x => x.Key.NameCmd == nameCommandOld)
                .FirstOrDefault(x => x.Key.NameDevice == nameDeviceOld).Key;

            if (DeviceCommands.ContainsKey(select))
            {
                var tempCmd = new MeterCmd();
                tempCmd.Transmit = transmitCmdNew;
                tempCmd.Delay = delayCmdNew;
                tempCmd.MessageType = typeNew;
                tempCmd.Receive = receiveCmdNew;
                tempCmd.StartOfString = startOfStringNew;
                tempCmd.EndOfString = endOfStringNew;
                tempCmd.PingCount = pingCountNew;

                if (string.IsNullOrWhiteSpace(transmitCmdNew))
                {
                    tempCmd.Transmit = DeviceCommands[select].Transmit;
                    Console.WriteLine(
                        $"Не Не была изменена команда {select.NameCmd} - {transmitCmdNew}");
                    //уведомить
                }

                if (string.IsNullOrWhiteSpace(receiveCmdNew))
                {
                    tempCmd.Receive = DeviceCommands[select].Receive;
                    Console.WriteLine(
                        $"Не была изменена команда {select.NameCmd} - {receiveCmdNew}");
                    //уведомить
                }

                if (delayCmdNew == 0)
                {
                    tempCmd.Delay = DeviceCommands[select].Delay;
                    Console.WriteLine(
                        $"Не была изменена команда {select.NameCmd} - {delayCmdNew}");
                    //уведомить
                }

                if (typeNew == TypeCmd.Text)
                {
                    tempCmd.MessageType = DeviceCommands[select].MessageType;
                    Console.WriteLine(
                        $"Не была изменена команда {select.NameCmd} - {typeNew}");
                    //уведомить
                }


                if (string.IsNullOrWhiteSpace(startOfStringNew))
                {
                    tempCmd.StartOfString = DeviceCommands[select].StartOfString;
                    Console.WriteLine(
                        $"Не была изменена команда {select.NameCmd} - {startOfStringNew}");
                    //уведомить
                }

                if (string.IsNullOrWhiteSpace(endOfStringNew))
                {
                    tempCmd.EndOfString = DeviceCommands[select].EndOfString;
                    Console.WriteLine(
                        $"Не была изменена команда {select.NameCmd} - {endOfStringNew}");
                    //уведомить
                }

                if (pingCountNew == 0)
                {
                    tempCmd.PingCount = DeviceCommands[select].PingCount;
                    Console.WriteLine(
                        $"Не была изменена команда {select.NameCmd} - {typeNew}");
                    //уведомить
                }

                DeviceCommands[select] = tempCmd;

                Console.WriteLine(
                    $"была изменена команда {select.NameCmd} для прибора тип {select.NameDevice}, имя прибора {select.NameDevice}");
                //уведомить
            }
        }
    }
}