using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Vips
{
    public class Stand
    {
        private MainValidator mainValidator = new MainValidator();
        public ObservableCollection<VoltMeter> VoltMeters { get; set; } = new ObservableCollection<VoltMeter>();
        public ObservableCollection<Thermometer> Thermometers { get; set; } = new ObservableCollection<Thermometer>();
        public ObservableCollection<Load> Loads { get; set; } = new ObservableCollection<Load>();

        public ObservableCollection<RelaySwitch> Relays { get; set; } = new ObservableCollection<RelaySwitch>();
        //public ObservableCollection<Vip> Vips { get; set; } = new ObservableCollection<Vip>();


        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// Добавление устройств в стенд
        /// </summary>
        /// <param name="typeDevice">Тип устройства</param>
        /// <param name="nameDevice">Имя устройства</param>
        /// <param name="portNum">Номер порта устройства</param>
        /// <param name="baudRate">Бауд Рейт устройства</param>
        /// <param name="stopBits">Стоповый Бит устройства</param>
        /// <param name="delayBetween">Задержка между переключенииями устройств</param>
        /// <param name="checkedOnConnectTimes">Количество проверок на коннект</param>
        /// <exception cref="DeviceException"></exception>
        public void AddDevice(TypeDevice typeDevice, string nameDevice, int portNum, int baudRate, int stopBits,
            int checkedOnConnectTimes = 1, int delayBetween = 0)
        {
            if (!mainValidator.ValidateCollisionPort(portNum))
            {
                throw new DeviceException($"Такой порт - {portNum} уже занят");
            }

            if (typeDevice == TypeDevice.VoltMeter)
            {
                var device = new VoltMeter
                {
                    Name = nameDevice,
                    //DelayBetween = delayBetween
                };
                if (!device.Config(portNum, baudRate, stopBits, checkedOnConnectTimes))
                {
                    throw new DeviceException($"Вольтметр {device.Name}, нет ответа");
                }

                VoltMeters.Add(device);
                Console.WriteLine($"Вольтметр {device.Name} был добавлен в стенд");
                //уведомитиь
            }

            if (typeDevice == TypeDevice.Thermometer)
            {
                var device = new Thermometer
                {
                    Name = nameDevice,
                    //DelayBetween = delayBetween
                };
                if (!device.Config(portNum, baudRate, stopBits, checkedOnConnectTimes))
                {
                    throw new DeviceException($"Термометр {device.Name}, нет ответа");
                }

                Thermometers.Add(device);
                Console.WriteLine($"Термометр {device.Name} был добавлен в стенд");
                //уведомить
            }

            if (typeDevice == TypeDevice.Load)
            {
                var device = new Load
                {
                    Name = nameDevice,
                    //DelayBetween = delayBetween
                };
                if (!device.Config(portNum, baudRate, stopBits, checkedOnConnectTimes))
                {
                    throw new DeviceException($"Нагрузка {device.Name}, нет ответа");
                }

                Loads.Add(device);
                Console.WriteLine($"Нагрузка {device.Name} была добавлена в стенд");
                //уведомитть
            }

            if (typeDevice == TypeDevice.Relay)
            {
                var device = new RelaySwitch
                {
                    Name = nameDevice,
                    DelayBetween = delayBetween
                };
                if (!device.Config(portNum, baudRate, stopBits, checkedOnConnectTimes))
                {
                    throw new DeviceException($"Реле {device.Name}, нет ответа");
                }

                Relays.Add(device);
                Console.WriteLine($"Реле {device.Name} было добавлено в стенд");
                //уведомить
            }

            mainValidator.BusyPorts.Add(portNum);
        }

        public void AddVips(ObservableCollection<Vip> configVipsVips)
        {
            for (int i = 0; i < configVipsVips.Count; i++)
            {
                try
                {
                    Relays[i].TestVip = configVipsVips[i];
                    Console.WriteLine(
                        $"Вип {configVipsVips[i].Name}, был добавлен к релейной плате {Relays[i].Name}, его статус" +
                        $" {configVipsVips[i].Status}");
                    //уведомить
                }
                catch (Exception e)
                {
                    throw new DeviceException(
                        $"Произошла ошибка {e} добавления Випа к релейному модулю{Relays[i].Name}");
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
            foreach (var relay in Relays)
            {
                if (relay.TestVip == null)
                {
                    //TODO если вип не доступен отключаем кнопку привязаную к релейной плате или к випу
                    Console.WriteLine($"К релейному модулю {relay.Name}, не подключен Вип");
                    //уведомить
                    return;
                }

                try
                {
                    int delay = 0; //имитация задержки
                    var testVip = relay.TestVip;
                    var typeVip = relay.TestVip.Type;
                    testVip.Status = StatusVip.None;
                    
                    //оснвной поток выполнения
                    
                    //создаем Thread 1
                    {
                        //1 Влючить релейный модуль
                        //TODO delay внутри TransmitReceivedDefaultCmd застопорит поток и получится 
                        //TODO что я буду ждать сперва тут а потом ниже?
                        relay.TransmitReceivedDefaultCmd("On");
                    }
                    //TODO нужно чтобы на выолпнение команды прошло время в основном потоке?
                    //TODO нужно чтобы тут до создания 2 потока прошло время с ммоента команды
                    delay = relay.DelayBetween;
                    
                    
                    //ждем выполнения Thread 1 и delay, а пототм создаем Thread 2
                    {
                        //TODO нужно чтобы выполнялась парарельно 3 и 4 задачам (те использовать задержки из команды
                        //TODO  ненадо тк  delay = relay.DelayBetween; длиной ~ 2 сек перекроет их длительность
                        //TODO ну или можно сдлетаь условие елси вдуг задержка команы больше 2 сек, все стоп опять зуб
                        // 2 Прочитать и присовить данные из измерителей
                        testVip.CurrentIn =
                            ReadDouble(VoltMeters[0]
                                .TransmitReceivedDefaultCmd("Curr?")); //1 источник проверяет входной ток
                        testVip.CurrentIn = 300;

                        if (testVip.CurrentIn > typeVip.PrepareMaxCurrentIn)
                        {
                            testVip.Status = StatusVip.Error;
                            throw new DeviceException($"Вип{testVip.Name} не прошел предварительное испытание " +
                                                      $"выходное напряжение на {testVip.CurrentIn - typeVip.PrepareMaxCurrentIn}" +
                                                      " больше чем нужно");
                        }
                        //TODO нужно чтобы выполнялась парарельно 2 и 4 задачам
                        testVip.VoltageOut1 =
                            ReadDouble(VoltMeters[1]
                                .TransmitReceivedDefaultCmd("Volt?")); //2 источник проверяет выходное напряжение 1 канал
                        testVip.VoltageOut1 = 300;
                        if (testVip.VoltageOut1 > typeVip.PrepareMaxVoltageOut1)
                        {
                            testVip.Status = StatusVip.Error;
                            throw new DeviceException($"Вип{testVip.Name} не прошел предварительное испытание " +
                                                      $"выходное напряжение на {testVip.VoltageOut1 - typeVip.PrepareMaxVoltageOut1}" +
                                                      " больше чем нужно");
                        }
                        //TODO нужно чтобы выполнялась парарельно 2 и 3 задачам
                        testVip.VoltageOut2 =
                            ReadDouble(VoltMeters[2]
                                .TransmitReceivedDefaultCmd("Volt?")); //3 источник проверяет выходное напряжение 2 канал
                        testVip.VoltageOut2 = 300;
                        if (testVip.VoltageOut2 > typeVip.PrepareMaxVoltageOut2)
                        {
                            testVip.Status = StatusVip.Error;
                            throw new DeviceException($"Вип{testVip.Name} не прошел предварительное испытание " +
                                                      $"выходное напряжение на {testVip.VoltageOut2 - typeVip.PrepareMaxVoltageOut2}" +
                                                      " больше чем нужно");
                        }
                    }
                    //TODO тут будет большая задержка гдето секунды 2
                    delay = relay.DelayBetween;
                    Console.WriteLine($"Задержка между испытаниями {delay}");
                }


                catch (DeviceException e)
                {
                    throw new DeviceException(e.Message);
                }
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
        }


        //TODO как произвести проверку випов я не оч понимаю вдь нужно взять и сделать это через измерители а перключать все платкой релейной
        (bool, RelaySwitch) CheckedVip()
        {
            foreach (var relay in Relays)
            {
            }

            return (false, null);
        }

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
        private string[] invalidSymbols = new[] {"\n", "\r"};

        protected double ReadDouble(string str)
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
    }
}