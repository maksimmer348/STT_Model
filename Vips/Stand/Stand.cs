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
        public ObservableCollection<Vip> Vips { get; set; } = new ObservableCollection<Vip>();


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
                    DelayBetween = delayBetween
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
                    DelayBetween = delayBetween
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
                    DelayBetween = delayBetween
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

        public void StandPrepareTest()
        {
            for (int i = 0; i < Vips.Count; i++)
            {
                //TODO правильно ли я добавляю в релейную плату может наоборот надо ,
                //TODO и как поступать если випов на релейные платы не хватает
                //TODO обработка исключения наверно ту ненужна ибо реелйных всегд 12 а випов всегда >= 
                Relays[i].TestVip = Vips[i];
                Console.WriteLine($"в релейную плату {Relays[i].Name}, был добвлен - Вип {Vips[i].Name}");
                //уведомить

                //Предварительные Испытания
                //1 Влючить релейный модуль
                Relays[i].TransmitReceivedDefaultCmd("On");

                var tempVip = Vips[i];
                
                //2 Прочитать данные из измерителя
                //
                tempVip.CurrentIn =  ReadDouble(VoltMeters[0].TransmitReceivedDefaultCmd("Curr?"));//1 источник проверяет входной ток
                tempVip.VoltageOut1 = ReadDouble(VoltMeters[1].TransmitReceivedDefaultCmd("Volt?"));//2 источник проверяет выходное напряжение 1 канал
                tempVip.VoltageOut2 = ReadDouble(VoltMeters[2].TransmitReceivedDefaultCmd("Volt?")); //3 источник проверяет выходное напряжение 2 канал

                if (tempVip.CurrentIn < tempVip.Type.MaxCurrent)
                {
                    
                }
                
                Vips[i].CurrentIn = ReadDouble(VoltMeters[0].TransmitReceivedDefaultCmd("Curr?"));// 1 источник проверяет входное напряжение
                int delay = Relays[i].DelayBetween;

                Console.WriteLine($"Задержка {delay} номер Випа {i}");
            }
        }

        Vip TestTick(int vipIndex)
        {
            //Испытания
            //1 Влючить релейный модуль
            Relays[vipIndex].TransmitReceivedDefaultCmd("On");
            //2 Прочитать данные из Вольтметра
            // Vips[vipIndex].VoltageIn = VoltMeters.Where(v=> v.).TransmitReceivedDefaultCmd("Volt?");
            Vips[vipIndex].CurrentIn = VoltMeters[0].TransmitReceivedDefaultCmd("Curr?");
            //3 Прочитать данные из Термометра
            return new Vip();
        }

        public bool Start()
        {
            foreach (var relay in Relays)
            {
            }

            return true;
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
            if (!double.TryParse(receive, NumberStyles.Any, CultureInfo.InvariantCulture,out double i))
            {
                throw new DeviceException($"Значние {receive} не удалось привести к числу");
            }
            return i;
        }
    }
    
}