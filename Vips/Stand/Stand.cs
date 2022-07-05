using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Vips
{
    public class Stand
    {
        public ObservableCollection<VoltMeter> VoltMeters { get; set; } = new ObservableCollection<VoltMeter>();
        public ObservableCollection<Thermometer> Thermometers { get; set; } = new ObservableCollection<Thermometer>();
        public ObservableCollection<Load> Loads { get; set; } = new ObservableCollection<Load>();

        public ObservableCollection<RelaySwitch> Relays { get; set; } = new ObservableCollection<RelaySwitch>();
        public ObservableCollection<Vip> Vips { get; set; } = new ObservableCollection<Vip>();

        List<int> boundsPorts = new List<int>();

        /// <summary>
        /// Добавление устройств в стенд
        /// </summary>
        /// <param name="typeDevice">Тип устройства</param>
        /// <param name="nameDevice">Имя устройства</param>
        /// <param name="portNum">Номер порта устройства</param>
        /// <param name="baudRate">БаудРейт устройства</param>
        /// <param name="stopBits">СтоповыйБит устройства</param>
        public void AddDevice(TypeDevice typeDevice, string nameDevice, int portNum, int baudRate, int stopBits,
            int checkedTimes = 1)
        {
            if (boundsPorts.Contains(portNum))
            {
                //TODO спросить что сделать тут исключение?
                Console.WriteLine("Такой порт уже занят");
                //уведомить
                return;
            }

            if (typeDevice == TypeDevice.VoltMeter)
            {
                var device = new VoltMeter
                {
                    Name = nameDevice
                };
                if (!device.Config(portNum, baudRate, stopBits, checkedTimes))
                {
                    return;
                }

                VoltMeters.Add(device);
                Console.WriteLine($"устройство {device.Name} было добавлено в стенд");
                //уведомитиь
                
            }

            if (typeDevice == TypeDevice.Thermometer)
            {
                var device = new Thermometer
                {
                    Name = nameDevice
                };
                if (!device.Config(portNum, baudRate, stopBits, checkedTimes))
                {
                    return;
                }

                Thermometers.Add(device);
                Console.WriteLine($"устройство {device.Name} было добавлено в стенд");
                //уведомить
            }

            if (typeDevice == TypeDevice.Load)
            {
                var device = new Load
                {
                    Name = nameDevice
                };
                if (!device.Config(portNum, baudRate, stopBits, checkedTimes))
                {
                    return;
                }

                Loads.Add(device);
                Console.WriteLine($"устройство {device.Name} было добавлено в стенд");
                //уведомитть
            }

            if (typeDevice == TypeDevice.Relay)
            {
                var device = new RelaySwitch
                {
                    Name = nameDevice
                };
                //TODO убрать потом  if (!device.Config(portNum, baudRate, stopBits, checkedTimes, false)) false
                if (!device.Config(portNum, baudRate, stopBits, checkedTimes, false))
                {
                    return;
                }

                Relays.Add(device);
                Console.WriteLine($"устройство {device.Name} было добавлено в стенд");
                //уведомить
            }

            boundsPorts.Add(portNum);
            //TODO возможно уведомитьиь хотя наверное нет и вообще стоит ил делать так
        }


        public bool Start()
        {
            for (int i = 0; i < Vips.Count; i++)
            {
                //TODO правильно ли я добавляю в релейную плату может наоборот надо ,
                //TODO и как поступать если випов на релейные платы не хватает
                //TODO обработка исключения наверно ту ненужна ибо реелйных всегд 12 а випов всегда >= 
                Relays[i].TestVip = Vips[i];
                Console.WriteLine($"в релейную плату {Relays[i].Name}, был добвлен - Вип {Vips[i].Name}");
                //уведомить
            }

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
    }
}