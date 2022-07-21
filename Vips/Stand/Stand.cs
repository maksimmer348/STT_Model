using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using RJCP.IO.Ports;
using SerialPortLib;

namespace Vips
{
    public class Stand
    {
        //Stopwatch stopwatch = new();
        private MainValidator mainValidator = new();
        public ObservableCollection<BaseDevice> Devices { get; set; } = new();
        public ObservableCollection<BaseDevice> TempVerifiedDevices { get; set; } = new();
        public ObservableCollection<RelaySwitch> Relays { get; set; } = new();

        /// <summary>
        /// Добавление устройств в стенд
        /// </summary>
        /// <param name="portLib">Используема устройством библиотека com port</param>
        /// <param name="typeDevice">Тип устройства</param>
        /// <param name="nameDevice">Имя устройства</param>
        /// <param name="pornName">Номер порта устройства</param>
        /// <param name="baud">Бауд Рейт устройства</param>
        /// <param name="stopBits">Стоповый Бит устройства</param>
        /// <param name="parity">Parity bits</param>
        /// <param name="dataBits">Колво байт в команде</param>
        /// <param name="dtr">Включить 12 вольт в компорте</param>
        /// <exception cref="StandException">Такой компорт уже занят</exception>
        public void AddDevice(TypePort portLib, TypeDevice typeDevice, string nameDevice, string pornName, int baud,
            int stopBits, int parity, int dataBits, bool dtr = false)
        {
            try
            {
                if (!mainValidator.ValidateCollisionPort(pornName))
                {
                    throw new StandException($"Stand Exception: Такой порт - {pornName} уже занят");
                }

                if (typeDevice == TypeDevice.Supply)
                {
                    var device = new Supply(nameDevice, typeDevice);
                    ConfigDeviceParams config = new ConfigDeviceParams()
                    {
                        TypePort = portLib, PortName = pornName,
                        Baud = baud, StopBits = stopBits, Parity = parity, DataBits = dataBits, Dtr = dtr
                    };
                    device.Config = config;
                    device.ConfigDevice();
                    device.ConnectPort += OnCheckConnectPort;
                    device.ConnectDevice += OnCheckDevice;
                    device.Receive += Receive;
                    Devices.Add(device);
                    Console.WriteLine(
                        $"Stand message: Устройство {device.Type}, {device.Name} было предварительно добавлено в стенд");
                    //уведомитиь
                }

                if (typeDevice == TypeDevice.VoltMeter)
                {
                    var device = new VoltMeter(nameDevice, typeDevice);

                    ConfigDeviceParams config = new ConfigDeviceParams()
                    {
                        TypePort = portLib, PortName = pornName,
                        Baud = baud, StopBits = stopBits, Parity = parity, DataBits = dataBits, Dtr = dtr
                    };
                    device.Config = config;
                    device.ConfigDevice();
                    device.ConnectPort += OnCheckConnectPort;
                    device.ConnectDevice += OnCheckDevice;
                    device.Receive += Receive;
                    Devices.Add(device);
                    Console.WriteLine(
                        $"Stand message: Устройство {device.Type}, {device.Name} было  было предварительно добавлено в стенд");
                    //уведомитиь
                }

                if (typeDevice == TypeDevice.Thermometer)
                {
                    var device = new Thermometer(nameDevice, typeDevice);
                    ConfigDeviceParams config = new ConfigDeviceParams()
                    {
                        TypePort = portLib, PortName = pornName,
                        Baud = baud, StopBits = stopBits, Parity = parity, DataBits = dataBits, Dtr = dtr
                    };
                    device.Config = config;
                    device.ConfigDevice();
                    device.ConnectPort += OnCheckConnectPort;
                    device.ConnectDevice += OnCheckDevice;
                    device.Receive += Receive;
                    Devices.Add(device);
                    Console.WriteLine(
                        $"Stand message: Устройство {device.Type}, {device.Name} было  было предварительно добавлено в стенд");
                    //уведомить
                }

                if (typeDevice == TypeDevice.Load)
                {
                    var device = new Load(nameDevice, typeDevice);
                    ConfigDeviceParams config = new ConfigDeviceParams()
                    {
                        TypePort = portLib, PortName = pornName,
                        Baud = baud, StopBits = stopBits, Parity = parity, DataBits = dataBits, Dtr = dtr
                    };
                    device.Config = config;
                    device.ConfigDevice();
                    device.ConnectPort += OnCheckConnectPort;
                    device.ConnectDevice += OnCheckDevice;
                    device.Receive += Receive;
                    Devices.Add(device);
                    Console.WriteLine(
                        $"Stand message: Устройство {device.Type}, {device.Name} было  было предварительно добавлено в стенд");
                    //уведомитть
                }

                if (!mainValidator.ValidateCollisionPort(pornName))
                {
                    mainValidator.BusyPorts.Add(pornName);
                }
            }

            catch (StandException e)
            {
                throw new StandException(e.Message);
            }
        }


        public void ChangeDevice(int indexDevice, ConfigDeviceParams newConfig)
        {
            try
            {
                var oldConf = Devices[indexDevice].GetConfigDevice();
                if (newConfig.PortName != default)
                {
                    Devices[indexDevice].Config.PortName = newConfig.PortName;
                    
                }
                Devices[indexDevice].ConfigDevice();
            }
            catch (StandException e)
            {
                throw new StandException($"Stand Exception: Такого устройства нет в списке");
            }
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
        /// <exception cref="StandException">Такой компорт уже занят</exception>
        public void AddRelays(string pornName, int baud, int stopBits,
            int parity, int dataBits, int count = 12)
        {
            if (!mainValidator.ValidateCollisionPort(pornName))
            {
                throw new StandException($"Stand Exception: Такой порт - {pornName} уже занят");
            }

            for (int i = 1; i <= count; i++)
            {
                var device = new RelaySwitch($"{count}", TypeDevice.Relay);
                ConfigDeviceParams config = new ConfigDeviceParams()
                {
                    PortName = pornName, Baud = baud, StopBits = stopBits, Parity = parity, DataBits = dataBits
                };
                device.Config = config;
                device.ConfigDevice();

                //device.ConnectPort += OnConnectPortDelay;
                //device.Receive += OnReceiveDelay;
                Relays.Add(device);

                Console.WriteLine($"Stand message: Реле {device.Name} было добавлено в стенд");
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
        /// <param name="delay">Общая задержка проверки (по умолчанию 10)</param>
        /// <returns></returns>
        public async Task<List<BaseDevice>> CheckConnectPorts(int delay = 10)
        {
            foreach (var device in Devices)
            {
                
                device.PortConnect();
            }

            await Task.Delay(TimeSpan.FromMilliseconds(delay));
            //если после задержки в этом списке будут устройства не прошедшие проверку
            return ErrorDevice();
        }

        //TODO это ведь выполняется прарельно?
        /// <summary>
        /// Проверка устройств пингуются ли они
        /// </summary>
        /// <returns></returns>
        public async Task<List<BaseDevice>> CheckConnectDevices()
        {
            //список для задержек из приборов
            var delaysList = new List<int>();
            //временный список дефетктивынх приборов
            var tempErrorDevices = new List<BaseDevice>();

            foreach (var device in Devices)
            {
                //отправляем команду проверки на устройство
                device.CheckedConnectDevice();
                delaysList.Add(device.CmdDelay);
            }

            //используем самую большую задержку из всех проверяемых приборов
            var delay = Convert.ToDouble(delaysList?.Max());
            //ждем (если по прношесвтии этого времени в tempErrorDevices чтот появится значит проверка не прошла)
            
            await Task.Delay(TimeSpan.FromMilliseconds(100));

            tempErrorDevices = ErrorDevice();
            return tempErrorDevices;
        }

        /// <summary>
        /// Проверка устройтва командой
        /// </summary>
        /// <returns></returns>
        private List<BaseDevice> ErrorDevice()
        {
            if (TempVerifiedDevices == null)
            {
                return Devices.ToList();
            }

            //сравниваем 
            var tempErrorDevices = Devices.Except(TempVerifiedDevices).ToList();
            TempVerifiedDevices.Clear();
            //возвращаем список приборов не прошедших проверку
            return tempErrorDevices;
        }

        /// <summary>
        /// Проверка компорта свободный/несвободный
        /// </summary>
        public void OnCheckConnectPort(BaseDevice baseDevice, bool connect)
        {
            if (connect)
            {
                TempVerifiedDevices.Add(baseDevice);
            }
            else
            {
                //TODO возможно использовать событие 
                throw new StandException(
                    $"Stand Exception: ComPort {baseDevice.GetConfigDevice().PortName} - не отвечает");
            }
        }

        /// <summary>
        /// Проверка устройства отвечает/неотвечает 
        /// </summary>
        public void OnCheckDevice(BaseDevice baseDevice, bool check)
        {
            if (check)
            {
                TempVerifiedDevices.Add(baseDevice);
            }
            else
            {
                throw new StandException(
                    $"Stand message: Устройство {baseDevice.Type} - {baseDevice.Name},на порту" +
                    $" {baseDevice.GetConfigDevice().PortName} неверня команда");
            }
        }

        /// <summary>
        /// Прием от устройства ответа на запрос
        /// </summary>
        private void Receive(BaseDevice device, string receive)
        {
            //Console.WriteLine($"Stand message: Время работы программы {device.Name} : {//stopwatch.Elapsed.TotalMilliseconds} милисекунд");
            //stopwatch.Restart(); // Остановить отсчет времени

            Console.WriteLine($"Stand message: Устройство {device.Name} отправил сообщение {receive}");
        }
        //TODO разобратся как это делать
        public void AddVips(ObservableCollection<Vip> configVipsVips)
        {
            for (int i = 0; i < configVipsVips.Count; i++)
            {
                try
                {
                    Relays[i].TestVip = configVipsVips[i];
                    Console.WriteLine(
                        $"Stand message: Вип {configVipsVips[i].Name}, был добавлен к релейной плате {Relays[i].TestVip.Name}, его статус" +
                        $" {configVipsVips[i].Status}");
                    //уведомить
                }
                catch (Exception e)
                {
                    throw new StandException(
                        $"Stand Exception: Произошла ошибка {e} добавления Випа к релейному модулю");
                }
            }
        }


        // private ModuleTestVip testModule;
        /// <summary>
        /// Предварительная задержка
        /// </summary>
        /// <exception cref="StandException">Куча всего может пойти не так оааоао</exception>
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
            //             throw new StandException($"Вип{testVip.Name} не прошел предварительное испытание " +
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
            //             throw new StandException($"Вип{testVip.Name} не прошел предварительное испытание " +
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
            //             throw new StandException($"Вип{testVip.Name} не прошел предварительное испытание " +
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
            // catch (StandException e)
            // {
            //     throw new StandException(e.Message);
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
                throw new StandException($"Stand Exception: Значние {receive} не удалось привести к числу");
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