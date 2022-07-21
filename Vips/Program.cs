// See https://aka.ms/new-console-template for more information

using System;
using System.Diagnostics;
using RJCP.IO.Ports;
using SerialPortLib;
using Vips;


Stand stand = new Stand();
ConfigVips configVips = new ConfigVips();


//configVips.ChangedTypeVips(0, new TypeVip() {MaxVoltageOut1 = 100, MaxVoltageOut2 = 200});
//int portNum = 1;
//добавление приборв измерения в стенд
//1 источник проверяет входной ток и входное напряжение
List<string> NameDevices = new List<string>() {"GDM-78255A", "PSW7-800-2.88"};
//                                                                                           "COM32"
stand.AddDevice(TypePort.SerialInput, TypeDevice.Supply, NameDevices[0], "COM39", 115200, 1, 0, 8);
//stand.AddDevice(TypePort.GodSerial, TypeDevice.Supply, "PSP-405", "COM33", 2400, 1, 0, 8, true);
stand.AddDevice(TypePort.SerialInput, TypeDevice.Supply, NameDevices[1], "COM34", 115200, 1, 0, 8);
BaseLibCmd libCmd = BaseLibCmd.getInstance();

foreach (var name in NameDevices)
{
    var status = libCmd.DeviceCommands.FirstOrDefault(x => x.Key.NameCmd == "Status" && x.Key.NameDevice == name).Value;
    if (status == null)
    {
        libCmd.AddCommand("Status", "GDM-78255A", "*IN?", "78255", 100);
    }
}

//добвление эфемерных випов
// configVips.AddVip("2", 0);
// configVips.AddVip("3", 0);
// configVips.AddVip("4", 0);
// configVips.AddVip("5", 0);
// configVips.AddVip("6", 0);
//
// string portNumRelays = "COM31";
// int baud = 960;
// int stopBitsRelays = 1;
// int parityRelays = 0;
// int dataBitsRelays = 8;
// int countRelays = 12;
// stand.AddRelays(portNumRelays, baud, stopBitsRelays, parityRelays, dataBitsRelays, countRelays);
// stand.AddVips(configVips.Vips);


//TODO когда нажали кнопку ведущую из конфига устройств делать ее disabled
Console.WriteLine("Programm message: Выполнить проверку устройств?");

//количетсво проверок приборов
int checkCount = 2;
Stopwatch stopwatch = new Stopwatch();

stopwatch.Restart();
//производим проверку компортов

//для отладки (добавление команды в битблиотеку)
libCmd.ChangeCommand("Status", "GDM-78255A", "*IDN?");
stand.ChangeDevice(0, new ConfigDeviceParams() {PortName = "COM32"});
for (int i = 1; i <= checkCount; i++)
{
    //принимает сбойные компорты
    List<BaseDevice> errorPortsList = await stand.CheckConnectPorts();
    //если сбоынйе компорты есть 
    if (errorPortsList.Any())
    {
        //перербираем их
        foreach (var errorDevice in errorPortsList)
        {
            Console.WriteLine(
                $"Programm message: Порт {errorDevice.GetConfigDevice().PortName} для устройства - {errorDevice.Name} НЕ открыт, попытка - {i}");
        }

        //сравниваем сбоынйе компорты со всеми в стенде
        var NoErrorPortsList = stand.Devices.Except(errorPortsList).ToList();
        //если в стенде есть рабочие компорты преебираем их
        foreach (var noErrorDevice in NoErrorPortsList)
        {
            Console.WriteLine(
                $"Programm message: Порт {noErrorDevice.GetConfigDevice().PortName} для устройства - {noErrorDevice.Name} попытка - {i}");
        }
    }

    //если сбоынйх компортов нет 
    if (!errorPortsList.Any())
    {
        //преребор провереных копортов
        foreach (var device in stand.Devices)
        {
            Console.WriteLine(
                $"Programm message: Порт {device.GetConfigDevice().PortName} для устройства - {device.Name} открыт, попытка - {i}");
        }

        //преходим к проверке устройств
        for (int j = 1; j <= checkCount; j++)
        {
            //принимает сбойные устройства
            List<BaseDevice> errorDevicesList = await stand.CheckConnectDevices();
            //если сбойные утсройства есть 
            if (errorDevicesList.Any())
            {
                //перебираем сбоынйх устройств
                foreach (var VARIABLE in errorDevicesList)
                {
                    Console.WriteLine($"Programm message: Устройство НЕ прошло - {VARIABLE.Name}, попытка - {j}");
                }

                //для отладки (добавление команды в битблиотеку)
                libCmd.ChangeCommand("Status", "GDM-78255A", "*IDN?");

                //сравниваем есть ли приборы без ошибок 
                var NoErrorDevicesList = stand.Devices.Except(errorDevicesList).ToList();
                //перебриаем приборы без ошибок
                foreach (var noErrorDevice in NoErrorDevicesList)
                {
                    Console.WriteLine(
                        $"Programm message: Устройство {noErrorDevice.Name} прошло попытка - {j}");
                }
            }

            //если приборов с ошибками нет
            if (!errorDevicesList.Any())
            {
                //перебриаем их
                foreach (var VARIABLE in stand.Devices)
                {
                    Console.WriteLine(
                        $"Programm message: Устройство прошло - {VARIABLE.Name}, попытка - {j}");
                }
                
                //TODO Время выполнения - 447 мс уменшить
                Console.WriteLine($"Programm message: Время выполнения - {stopwatch.ElapsedMilliseconds} мс");

                //и начинаем проверку реле
                //stand.CheckConnectRelays();
                return;
            }
        }
    }

    stand.ChangeDevice(0, new ConfigDeviceParams() {PortName = "COM32"});
}

Console.Read();

// bool connectLoop = true;
// List<BaseDevice> errorDevicesList = new List<BaseDevice>();
// while (connectLoop)
// {
//     errorDevicesList = await stand.CheckConnectPort();
//     
//     if (errorDevicesList.Any())
//     {
//         foreach (var errorDevice in errorDevicesList)
//         {
//             Console.WriteLine(
//                 $"Порт {errorDevice.GetConfigDevice().PortName} для устройства - {errorDevice.Name} не открыт");
//         }
//     }
//
//     Console.WriteLine("Повторить попытку");
//     var r = Console.ReadLine();
//     if (r == "Q".ToLower())
//     {
//         connectLoop = false;
//     }
// }
//
// var receiveLoop = true;
//
// while (receiveLoop)
// {
//     errorDevicesList = await stand.CheckConnectDevices();
//     if (!errorDevicesList.Any())
//     {
//         foreach (var VARIABLE in stand.Devices)
//         {
//             Console.WriteLine($"прошло - {VARIABLE.Name}");
//         }
//
//         stand.StandPrepareTest();
//     }
// }


//var devicesL = await stand.CheckConnectPort(1);

//stand.StandPrepareTest();
//TODO првильно ли я все написал (надо чтобы обновлялось configVips.Vips, когда измененяется stand.Vips)
//Если все ок добавляем Випы из конфигуратора в стенд 
//stand.Vips = configVips.Vips;
//посе этого присваиваем каждому Випу его платку
//stand.Start();