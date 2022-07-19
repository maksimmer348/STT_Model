// See https://aka.ms/new-console-template for more information

using System;
using RJCP.IO.Ports;
using SerialPortLib;
using Vips;


Stand stand = new Stand();
ConfigVips configVips = new ConfigVips();

// Console.WriteLine("введите номер випа");
// string wrN = Console.ReadLine();
// Console.WriteLine("введите тип випа");
// string wrT = Console.ReadLine();
// configVips.AddVip(wrN, Int32.Parse(wrT));
//
// Console.WriteLine("введите номер випа");
// w
// rN = Console.ReadLine();
// configVips.AddVip(wrN, Int32.Parse(wrT));
//
// Console.WriteLine("введите номер випа");
// wrN = Console.ReadLine();
// configVips.AddVip(wrN, Int32.Parse(wrT));
//
// Console.WriteLine("введите номер випа");
// wrN = Console.ReadLine();
// configVips.AddVip(wrN, Int32.Parse(wrT));

//добвление эфемерных випов
// configVips.AddVip("1", 0);
// configVips.AddVip("2", 0);
// configVips.AddVip("3", 0);
// configVips.AddVip("4", 0);
// configVips.AddVip("5", 0);
// configVips.AddVip("6", 0);

//configVips.ChangedTypeVips(0, new TypeVip() {MaxVoltageOut1 = 100, MaxVoltageOut2 = 200});
//int portNum = 1;
//добавление приборв измерения в стенд
//1 источник проверяет входной ток и входное напряжение
stand.AddDevice(TypePort.SerialInput,TypeDevice.VoltMeter, "GDM-78255A", "COM32", 115200, 1, 0, 8);
stand.AddDevice(TypePort.GodSerial,TypeDevice.VoltMeter, "PSP-405", "COM33", 2400,1,0,8, true);
stand.AddDevice(TypePort.SerialInput,TypeDevice.VoltMeter, "PSW7-800-2.88", "COM34", 115200, 1, 0, 8);
//2 источник проверяет выходное напряжение 1 канал
//stand.AddDevice(TypeDevice.VoltMeter, "GPS-74303", portNum++, 0, 8,0);

//3 источник проверяет выходное напряжение 2 канал
//stand.AddDevice(TypeDevice.VoltMeter, "GPS-74303", portNum++, 0, 8, 0);
    
//1 термометр проверяет температуру
//stand.AddDevice(TypeDevice.Thermometer, "GDM-7433",portNum++ , 0, 8,0);
//1 нагрузка нагружает выбранный ВИП
//stand.AddDevice(TypeDevice.Load, "GDM-4303",portNum++ , 0, 8,0);
string portNumRelays = "COM31";
//stand.AddRelays(portNumRelays , 9600, 1,0, 8, 12);
//stand.AddDevice(TypeDevice.Relay, "2", portNumRelays, 9600, stopBitsRelays,parityRelays, dataBitsRelays);
// stand.AddDevice(TypeDevice.Relay, "3", portNumRelays, 9600, stopBitsRelays,parityRelays, dataBitsRelays);
// stand.AddDevice(TypeDevice.Relay, "4", portNumRelays, 9600, stopBitsRelays,parityRelays, dataBitsRelays);
// stand.AddDevice(TypeDevice.Relay, "5", portNumRelays, 9600, stopBitsRelays,parityRelays, dataBitsRelays);
// stand.AddDevice(TypeDevice.Relay, "6", portNumRelays, 9600, stopBitsRelays,parityRelays, dataBitsRelays);
// stand.AddDevice(TypeDevice.Relay, "7", portNumRelays, 9600, stopBitsRelays,parityRelays, dataBitsRelays);
// stand.AddDevice(TypeDevice.Relay, "8", portNumRelays, 9600, stopBitsRelays,parityRelays, dataBitsRelays);
// stand.AddDevice(TypeDevice.Relay, "9", portNumRelays, 9600, stopBitsRelays,parityRelays, dataBitsRelays);
// stand.AddDevice(TypeDevice.Relay, "10", portNumRelays, 9600, stopBitsRelays,parityRelays, dataBitsRelays);
// stand.AddDevice(TypeDevice.Relay, "11", portNumRelays, 9600, stopBitsRelays,parityRelays, dataBitsRelays);
// stand.AddDevice(TypeDevice.Relay, "12", portNumRelays, 9600, stopBitsRelays,parityRelays, dataBitsRelays);
//stand.AddVips(configVips.Vips);
stand.Cnn();
while (true)
{
    stand.NCheck();
}


return;

var devicesList =  await stand.CheckConnectPort();
devicesList = await stand.CheckConnectDevices();
return;

// if (!devicesList.Any())
// {
//     devicesList = await stand.CheckConnectDevices();
//     if (!devicesList.Any())
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
