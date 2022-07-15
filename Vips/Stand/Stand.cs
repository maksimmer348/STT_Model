using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using RJCP.IO.Ports;
using SerialPortLib;

namespace Vips
{
    public class Stand
    {
        private MainValidator mainValidator = new MainValidator();
        public ObservableCollection<BaseMeter> Devices { get; set; } = new ObservableCollection<BaseMeter>();

        public ObservableCollection<BaseMeter> TempVerifiedDevices { get; set; } =
            new ObservableCollection<BaseMeter>();

        public ObservableCollection<RelaySwitch> Relays { get; set; } = new ObservableCollection<RelaySwitch>();

        private event Action<ObservableCollection<BaseMeter>> ConnectDevicesStatus;

        public Stand()
        {
            ConnectDevicesStatus += OnErrorConnectDevices;
        }

        public void OnErrorConnectDevices(ObservableCollection<BaseMeter> obj)
        {
            foreach (var meter in obj)
            {
                Console.WriteLine($"Устройство {meter.Name} на порту {meter.GetPortNum} не функционирует");
            }
        }

        /// <summary>
        /// Добавление устройств в стенд
        /// </summary>
        /// <param name="typeDevice">Тип устройства</param>
        /// <param name="nameDevice">Имя устройства</param>
        /// <param name="pornName">Номер порта устройства</param>
        /// <param name="baud">Бауд Рейт устройства</param>
        /// <param name="stopBits">Стоповый Бит устройства</param>
        /// <param name="parity">Parity bits</param>
        /// <param name="dataBits">Колво байт в команде</param>
        /// <param name="dtr">Включить 12 вольт в компорте</param>
        /// <exception cref="DeviceException">Такой компорт уже занят</exception>
        public void AddDevice(TypeDevice typeDevice, string nameDevice, string pornName, int baud, StopBits stopBits,
            Parity parity, DataBits dataBits, bool dtr = false)
        {
            if (!mainValidator.ValidateCollisionPort(pornName))
            {
                throw new DeviceException($"Такой порт - {pornName} уже занят");
            }

            if (typeDevice == TypeDevice.VoltMeter)
            {
                var device = new VoltMeter
                {
                    Type = TypeDevice.VoltMeter,
                    Name = nameDevice,
                };
                device.Config(pornName, baud, stopBits, parity, dataBits, dtr);
                device.ConnectPort += OnConnectPort;
                device.ConnectDevice += OnCheckCmdDevice;
                device.Receive += OnReceive;

                Devices.Add(device);
                Console.WriteLine($"Устройство {device.Type}, {device.Name} было добавлена в стенд");
                //уведомитиь
            }

            if (typeDevice == TypeDevice.Thermometer)
            {
                var device = new Thermometer
                {
                    Name = nameDevice,
                };
                device.Config(pornName, baud, stopBits, parity, dataBits, dtr);
                device.ConnectPort += OnConnectPort;
                device.ConnectDevice += OnCheckCmdDevice;
                device.Receive += OnReceive;

                Devices.Add(device);
                Console.WriteLine($"Устройство {device.Type}, {device.Name} было добавлена в стенд");
                //уведомить
            }

            if (typeDevice == TypeDevice.Load)
            {
                var device = new Load
                {
                    Name = nameDevice,
                };
                device.Config(pornName, baud, stopBits, parity, dataBits, dtr);
                device.ConnectPort += OnConnectPort;
                device.ConnectDevice += OnCheckCmdDevice;
                device.Receive += OnReceive;

                Devices.Add(device);
                Console.WriteLine($"Устройство {device.Type}, {device.Name} было добавлена в стенд");
                //уведомитть
            }

            //TODO для тестов
            if (typeDevice == TypeDevice.Supply)
            {
                var device = new Supply
                {
                    Type = TypeDevice.VoltMeter,
                    Name = nameDevice,
                };
                device.Config(pornName, baud, stopBits, parity, dataBits, dtr);
                device.ConnectPort += OnConnectPort;
                device.ConnectDevice += OnCheckCmdDevice;
                device.Receive += OnReceive;

                Devices.Add(device);
                Console.WriteLine($"Устройство {device.Type}, {device.Name} было добавлена в стенд");
                //уведомитиь
            }
            //
        }

        /// <summary>
        /// Добавление релейных плат в стенд
        /// </summary>
        /// <param name="pornName">Номер порта устройства</param>
        /// <param name="baud">Бауд Рейт устройства</param>
        /// <param name="stopBits">Стоповый Бит устройства</param>
        /// <param name="parity">Parity bits</param>
        /// <param name="dataBits">Колво байт в команде</param>
        /// <param name="count">Общее колво релейных плат</param>
        /// <exception cref="DeviceException">Такой компорт уже занят</exception>
        public void AddRelays(string pornName, int baud, StopBits stopBits,
            Parity parity, DataBits dataBits, int count = 12)
        {
            if (!mainValidator.ValidateCollisionPort(pornName))
            {
                throw new DeviceException($"Такой порт - {pornName} уже занят");
            }

            for (int i = 1; i <= count; i++)
            {
                var device = new RelaySwitch
                {
                    Type = TypeDevice.Relay,
                    Name = $"{count}"
                };

                device.Config(pornName, baud, stopBits, parity, dataBits);
                device.ConnectPort += OnConnectPortDelay;
                device.ConnectDevice += OnCheckDelay;
                device.Receive += OnReceiveDelay;
                Relays.Add(device);

                Console.WriteLine($"Реле {device.Name} было добавлено в стенд");
                //уведомить
            }

            if (!mainValidator.ValidateCollisionPort(pornName))
            {
                mainValidator.BusyPorts.Add(pornName);
            }
        }

        /// <summary>
        /// Проверка на физическое существование порта  
        /// </summary>
        /// <param name="delay">Общая задержка проверки (по умолчанию 50)</param>
        /// <returns></returns>
        public async Task<List<BaseMeter>> CheckConnectPort(int delay = 50)
        {
            //TODO когда нажали кнопку делать ее disabled
            foreach (var device in Devices)
            {
                device.PortConnect();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(delay));
            return CheckDevice();
        }

        /// <summary>
        /// Проверка устройств не занят ли порт и пингуются ли они
        /// </summary>
        /// <param name="checkedOnConnectTimes">Количество проверок (по умолчанию 1)</param>
        /// <param name="delay">Общая задержка проверки (если 0 то используется самая большая из представленых прибороов)</param>
        /// <returns></returns>
        public async Task<List<BaseMeter>> CheckConnectDevices(int checkedOnConnectTimes = 2, int delay = 0)
        {
            //список для задержек из приборов
            var delaysList = new List<int>();
            //временный список дефетктивынх приборов
            var tempErrorDevices = new List<BaseMeter>();
            
            foreach (var device in Devices)
            {
                for (int i = 0; i < 3; i++)
                {
                    //отправляем команду проверки на устройство
                    device.CheckedConnectDevice();
                    
                    //добавлено для выбора самой большой задержки из приборов в общую задержку
                    delaysList.Add(device.Delay);
                    
                    //если общая задержка не указана
                    if (delay == 0)
                    {
                        //используем самую большую задержку из всех проверяемых приборов
                        delay = delaysList.Max();
                    }
                    //ждем (если по прношесвтии этого времени в tempErrorDevices чтот появится значит проверка не прошла)
                    await Task.Delay(TimeSpan.FromMilliseconds(delay));
                    tempErrorDevices = CheckDevice();
                    //TODO как сделать несолькок проверок если не проходит в первый раз с ожиданием (2 сек гденть)
                    // if (!tempErrorDevices.Any())
                    // {
                    //     break;
                    // }
                }
            }
            return tempErrorDevices;
        }
        
        /// <summary>
        /// Проверка устройтва командой
        /// </summary>
        /// <returns></returns>
        private List<BaseMeter> CheckDevice()
        {
            //сравниваем 
            var tempErrorDevices = Devices.Except(TempVerifiedDevices).ToList();
            TempVerifiedDevices.Clear();
            return tempErrorDevices;
        }
        /// <summary>
        /// Проверка компорта свободный/несвободный
        /// </summary>
        /// <returns></returns>
        public void OnConnectPort(BaseMeter baseMeter, bool connect)
        {
            if (connect)
            {
                TempVerifiedDevices.Add(baseMeter);
            }
            else
            {
                //TODO возможно использовать событие 
                throw new DeviceException($"Порт {baseMeter.GetPortNum} не отвечает");
            }
        }

        public void OnCheckCmdDevice(BaseMeter baseMeter, bool check)
        {
            if (check)
            {
                TempVerifiedDevices.Add(baseMeter);
            }
            else
            {
                throw new DeviceException(
                    $"Устройство {baseMeter.Type} - {baseMeter.Name},на порту {baseMeter.GetPortNum} неверня команда");
            }
        }

        public void OnReceive(BaseMeter arg1, byte[] receive)
        {
        }

        private void OnConnectPortDelay(BaseMeter arg1, bool arg2)
        {
            throw new NotImplementedException();
        }

        private void OnCheckDelay(BaseMeter arg1, bool arg2)
        {
            throw new NotImplementedException();
        }

        private void OnReceiveDelay(BaseMeter arg1, byte[] arg2)
        {
            throw new NotImplementedException();
        }

        public void AddVips(ObservableCollection<Vip> configVipsVips)
        {
            for (int i = 0; i < configVipsVips.Count; i++)
            {
                try
                {
                    Relays[i].TestVip = configVipsVips[i];
                    Console.WriteLine(
                        $"Вип {configVipsVips[i].Name}, был добавлен к релейной плате {Relays[i].TestVip.Name}, его статус" +
                        $" {configVipsVips[i].Status}");
                    //уведомить
                }
                catch (Exception e)
                {
                    throw new DeviceException(
                        $"Произошла ошибка {e} добавления Випа к релейному модулю");
                }
            }
        }


        // private ModuleTestVip testModule;
        /// <summary>
        /// Предварительная задержка
        /// </summary>
        /// <exception cref="DeviceException">Куча всего может пойти не так оааоао</exception>
        public void StandPrepareTest()
        {
            // foreach (var relay in Relays)
            // {
            //     if (relay.TestVip == null)
            //     {
            //         //TODO если вип не доступен отключаем кнопку привязаную к релейной плате или к випу
            //         Console.WriteLine($"К релейному модулю {relay.Name}, не подключен Вип");
            //         //уведомить
            //         return;
            //     }
            //
            //     try
            //     {
            //         //создаем Thread 1
            //         //1 Влючить релейный модуль 
            //         relay.TransmitReceivedDefaultCmd("On");
            //         CheckDevices(1);
            //     }
            //     catch (Exception e)
            //     {
            //         Console.WriteLine(e);
            //         throw;
            //     }
            // try
            // {
            //     var testVip = relay.TestVip;
            //     var typeVip = relay.TestVip.Type;
            //     testVip.Status = StatusVip.None;
            //
            //     //оснвной поток выполнения
            //
            //     //создаем Thread 1
            //     {
            //         //1 Влючить релейный модуль
            //         //TODO delay внутри TransmitReceivedDefaultCmd застопорит поток и получится 
            //         //TODO что я буду ждать сперва тут а потом ниже?
            //         relay.TransmitReceivedDefaultCmd("On");
            //     }
            //     //TODO нужно чтобы на выолпнение команды прошло время в основном потоке?
            //     //TODO нужно чтобы тут до создания 2 потока прошло время с ммоента команды
            //     //Thread.Sleep();
            //
            //
            //     //ждем выполнения Thread 1 и delay, а пототм создаем Thread 2
            //     {
            //         //TODO нужно чтобы выполнялась парарельно 3 и 4 задачам (те использовать задержки из команды
            //         //TODO  ненадо тк  delay = relay.DelayBetween; длиной ~ 2 сек перекроет их длительность
            //         //TODO ну или можно сдлетаь условие елси вдуг задержка команы больше 2 сек, все стоп опять зуб
            //         // 2 Прочитать и присовить данные из измерителей
            //         testVip.CurrentIn =
            //             ReadDouble(VoltMeters[0]
            //                 .TransmitReceivedDefaultCmd("Curr?")); //1 источник проверяет входной ток
            //         testVip.CurrentIn = 300;
            //
            //         if (testVip.CurrentIn > typeVip.PrepareMaxCurrentIn)
            //         {
            //             testVip.Status = StatusVip.Error;
            //             throw new DeviceException($"Вип{testVip.Name} не прошел предварительное испытание " +
            //                                       $"выходное напряжение на {testVip.CurrentIn - typeVip.PrepareMaxCurrentIn}" +
            //                                       " больше чем нужно");
            //         }
            //
            //         //TODO нужно чтобы выполнялась парарельно 2 и 4 задачам
            //         testVip.VoltageOut1 =
            //             ReadDouble(VoltMeters[1]
            //                 .TransmitReceivedDefaultCmd(
            //                     "Volt?")); //2 источник проверяет выходное напряжение 1 канал
            //         testVip.VoltageOut1 = 300;
            //         if (testVip.VoltageOut1 > typeVip.PrepareMaxVoltageOut1)
            //         {
            //             testVip.Status = StatusVip.Error;
            //             throw new DeviceException($"Вип{testVip.Name} не прошел предварительное испытание " +
            //                                       $"выходное напряжение на {testVip.VoltageOut1 - typeVip.PrepareMaxVoltageOut1}" +
            //                                       " больше чем нужно");
            //         }
            //
            //         //TODO нужно чтобы выполнялась парарельно 2 и 3 задачам
            //         testVip.VoltageOut2 =
            //             ReadDouble(VoltMeters[2]
            //                 .TransmitReceivedDefaultCmd(
            //                     "Volt?")); //3 источник проверяет выходное напряжение 2 канал
            //         testVip.VoltageOut2 = 300;
            //         if (testVip.VoltageOut2 > typeVip.PrepareMaxVoltageOut2)
            //         {
            //             testVip.Status = StatusVip.Error;
            //             throw new DeviceException($"Вип{testVip.Name} не прошел предварительное испытание " +
            //                                       $"выходное напряжение на {testVip.VoltageOut2 - typeVip.PrepareMaxVoltageOut2}" +
            //                                       " больше чем нужно");
            //         }
            //     }
            //     //TODO тут будет большая задержка гдето секунды 2
            //     delay = relay.DelayBetween;
            //     Console.WriteLine($"Задержка между испытаниями {delay}");
            // }
            //
            //
            // catch (DeviceException e)
            // {
            //     throw new DeviceException(e.Message);
            // }
        }

        static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

        //         Console.WriteLine($"в релейную плату {Relays[i].Name}, был добвлен - Вип {Vips[i].Name}");
        //         //уведомить
        //     
        //         //--Предварительные Испытания--
        //         

        //         var tempVip = Vips[i];
        //     
        //         //2 Прочитать данные из измерителя
        //         tempVip.CurrentIn = ReadDouble(VoltMeters[0].TransmitReceivedDefaultCmd("Curr?")); //1 источник проверяет входной ток
        //         tempVip.VoltageOut1 =
        //             ReadDouble(VoltMeters[1]
        //                 .TransmitReceivedDefaultCmd("Volt?")); //2 источник проверяет выходное напряжение 1 канал
        //         tempVip.VoltageOut2 =
        //             ReadDouble(VoltMeters[2]
        //                 .TransmitReceivedDefaultCmd("Volt?")); //3 источник проверяет выходное напряжение 2 канал
        //     
        //         if (tempVip.CurrentIn < tempVip.Type.MaxCurrent)
        //         {
        //         }
        //     
        //         // Vips[i].CurrentIn = ReadDouble(VoltMeters[0].TransmitReceivedDefaultCmd("Curr?"));// 1 источник проверяет входное напряжение
        //         
        //     
        //         Console.WriteLine($"Задержка {delay} номер Випа {i}");
        //     }
        // }
        //
        // Vip TestTick(int vipIndex)
        // {
        //     //Испытания
        //     //1 Влючить релейный модуль
        //     Relays[vipIndex].TransmitReceivedDefaultCmd("On");
        //     //2 Прочитать данные из Вольтметра
        //     // Vips[vipIndex].VoltageIn = VoltMeters.Where(v=> v.).TransmitReceivedDefaultCmd("Volt?");
        //     // Vips[vipIndex].CurrentIn = VoltMeters[0].TransmitReceivedDefaultCmd("Curr?");
        //     //3 Прочитать данные из Термометра
        //     return new Vip();
        // }
        //
        // public bool Start()
        // {
        //     foreach (var relay in Relays)
        //     {
        //     }
        //
        //     return true;
        string[] invalidSymbols = new[] {"\n", "\r"};

        double ReadDouble(string str)
        {
            var receive = str;
            foreach (var t in invalidSymbols)
            {
                receive = receive.Replace(t, "");
            }

            if (!double.TryParse(receive, NumberStyles.Any, CultureInfo.InvariantCulture, out double i))
            {
                //TODO приемлимо ли так
                throw new DeviceException($"Значние {receive} не удалось привести к числу");
            }

            return i;
        }

        // //TODO как произвести проверку випов я не оч понимаю вдь нужно взять и сделать это через измерители а перключать все платкой релейной
// (bool, RelaySwitch) CheckedVip()
// {
//     foreach (var relay in Relays)
//     {
//     }
//
//     return (false, null);
// }

// bool CheckedVip()
// {
//     if (Temperature > Type.MaxTemperature || Voltage > Type.MaxVoltage || Current > Type.MaxCurrent)
//     {
//         Status = StatusVip.Error;
//         return false;
//     }
//
//     Status = StatusVip.Ok;
//     return true;
// }

//TODO продолжить список
//TODO уточнить название 

//void ConnectPort(BaseMeter arg1, bool arg2)
        // {
        //     if (arg2)
        //     {
        //         // if (arg1.Type == TypeDevice.VoltMeter)
        //         // {
        //         //     
        //         // }
        //         // if (arg1.Type == TypeDevice.Thermometer)
        //         // {
        //         //     
        //         // }
        //         // if (arg1.Type == TypeDevice.Load)
        //         // {
        //         //     
        //         // }
        //         ConnectedPorts.Add(arg1);
        //     }
        // }
        //
        // void CheckDevice(BaseMeter arg1, bool arg2)
        // {
        //     if (arg2)
        //     {
        //         ConnectedDevices.Add(arg1);
        //     }
        // }
        //
        // void Receive(byte[] obj)
        // {
        //     throw new NotImplementedException();
        // }


        // public BaseMeter Check()
        // {
        //     Devices.Where()
        // }
        //
    }
}