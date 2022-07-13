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
        public ObservableCollection<BaseMeter> tempDevices { get; set; } = new ObservableCollection<BaseMeter>();

        public ObservableCollection<RelaySwitch> Relays { get; set; } = new ObservableCollection<RelaySwitch>();
        //public ObservableCollection<VoltMeter> VoltMeters { get; set; } = new ObservableCollection<VoltMeter>();
        //public ObservableCollection<Thermometer> Thermometers { get; set; } = new ObservableCollection<Thermometer>();
        //public ObservableCollection<Load> Loads { get; set; } = new ObservableCollection<Load>();
        //public ObservableCollection<Vip> Vips { get; set; } = new ObservableCollection<Vip>();
        //public ObservableCollection<BaseMeter> ConnectedPorts { get; set; } = new ObservableCollection<BaseMeter>();
        //public ObservableCollection<BaseMeter> ConnectedDevices { get; set; } = new ObservableCollection<BaseMeter>();
        //protected ManualResetEvent Waiting = new ManualResetEvent(true);

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
        /// <param name="checkedOnConnectTimes">Количество проверок на коннект</param>
        /// <param name="delayBetween">Задержка между переключенииями устройств</param>
        /// <exception cref="DeviceException"></exception>
        public void AddDevice(TypeDevice typeDevice, string nameDevice, string pornName, int baud, StopBits stopBits,
            Parity parity, DataBits dataBits)
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
                device.Config(pornName, baud, stopBits, parity, dataBits);
                device.ConnectPort += OnConnectPort;
                device.ConnectDevice += OnCheckDevice;
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
                device.Config(pornName, baud, stopBits, parity, dataBits);
                device.ConnectPort += OnConnectPort;
                device.ConnectDevice += OnCheckDevice;
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
                device.Config(pornName, baud, stopBits, parity, dataBits);
                device.ConnectPort += OnConnectPort;
                device.ConnectDevice += OnCheckDevice;
                device.Receive += OnReceive;

                Devices.Add(device);
                Console.WriteLine($"Устройство {device.Type}, {device.Name} было добавлена в стенд");
                //уведомитть
            }
        }

        public void AddRelays(TypeDevice typeDevice, string pornName, int baud, StopBits stopBits,
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
                //TODO пернести в отдельный метод
                // if (!device.Config(portNum, baudRate, stopBits, checkedOnConnectTimes))
                // {
                //throw new RelayException($"Реле {device.Name}, нет ответа");
                // }

                Console.WriteLine($"Реле {device.Name} было добавлено в стенд");
                //уведомить
            }

            if (!mainValidator.ValidateCollisionPort(pornName))
            {
                mainValidator.BusyPorts.Add(pornName);
            }
        }

        public void CheckConnectPort(int checkedOnConnectTimes, int delay = 50)
        {
            foreach (var device in Devices)
            {
                device.PortConnect();

                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(delay));
                    CheckConnectDevice();
                }).ConfigureAwait(false);
            }
            tempDevices.Clear();
        }

        public void CheckConnectDevices(int checkedOnConnectTimes, int delay = 150)
        {
            foreach (var device in Devices)
            {
                device.CheckedConnect(checkedOnConnectTimes);

                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(delay));
                    CheckConnectDevice();
                }).ConfigureAwait(false);
            }
            tempDevices.Clear();
        }

        private void CheckConnectDevice()
        {
           var devices =  Devices.Except(tempDevices).ToList();
           if (devices.Any())
           {
               foreach (var device in devices)
               {
                   throw new DeviceException($"Устройство {device.Name} на порту {device.GetPortNum} не отвечает");
               }
           }
        }

        public void OnConnectPort(BaseMeter baseMeter, bool connect)
        {
            if (connect)
            {
                tempDevices.Add(baseMeter);
            }
            else
            {
                throw new DeviceException($"Порт {baseMeter.GetPortNum} не отвечает");
            }
        }

        public void OnCheckDevice(BaseMeter baseMeter, bool check)
        {
            if (check)
            {
                tempDevices.Add(baseMeter);
            }
            else
            {
                throw new DeviceException($"Устройство {baseMeter.Type} - {baseMeter.Name},на порту {baseMeter.GetPortNum} неверня команда");
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