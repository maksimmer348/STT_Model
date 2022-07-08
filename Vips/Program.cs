// See https://aka.ms/new-console-template for more information

using System;
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
// wrN = Console.ReadLine();
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
configVips.AddVip("1", 0);
configVips.AddVip("2", 0);
configVips.AddVip("3", 0);
configVips.AddVip("4", 0);
configVips.AddVip("5", 0);
configVips.AddVip("6", 0);

//configVips.ChangedTypeVips(0, new TypeVip() {MaxVoltageOut1 = 100, MaxVoltageOut2 = 200});
int portNum = 1;
//добавление приборв измерения в стенд
//1 источник проверяет входной ток и входное напряжение
stand.AddDevice(TypeDevice.VoltMeter, "GPS-74303", portNum++, 0, 8,1);
//2 источник проверяет выходное напряжение 1 канал
stand.AddDevice(TypeDevice.VoltMeter, "GPS-74303", portNum++, 0, 8,0);
//3 источник проверяет выходное напряжение 2 канал
stand.AddDevice(TypeDevice.VoltMeter, "GPS-74303", portNum++, 0, 8, 0);
    
//1 термометр проверяет температуру
stand.AddDevice(TypeDevice.Thermometer, "GDM-7433",portNum++ , 0, 8,0);
//1 нагрузка нагружает выбранный ВИП
stand.AddDevice(TypeDevice.Load, "GDM-4303",portNum++ , 0, 8,0);

stand.AddDevice(TypeDevice.Relay, "1", portNum++, 0, 8,0, 2000);
stand.AddDevice(TypeDevice.Relay, "2", portNum++, 0, 8,0, 2000);
stand.AddDevice(TypeDevice.Relay, "3", portNum++, 0, 8,0, 2000);
stand.AddDevice(TypeDevice.Relay, "4", portNum++, 0, 8,0, 2000);
stand.AddDevice(TypeDevice.Relay, "5", portNum++, 0, 8,0, 2000);
stand.AddDevice(TypeDevice.Relay, "6", portNum++, 0, 8,0, 2000);
stand.AddDevice(TypeDevice.Relay, "7", portNum++, 0, 8,0, 2000);
stand.AddDevice(TypeDevice.Relay, "8", portNum++, 0, 8,0, 2000);
stand.AddDevice(TypeDevice.Relay, "9", portNum++, 0, 8,0, 2000);
stand.AddDevice(TypeDevice.Relay, "10", portNum++, 0, 8,0, 2000);
stand.AddDevice(TypeDevice.Relay, "11", portNum++, 0, 8,0, 2000);
stand.AddDevice(TypeDevice.Relay, "12", portNum++, 0, 8,0, 2000);

stand.AddVips(configVips.Vips);
stand.StandPrepareTest();
//TODO првильно ли я все написал (надо чтобы обновлялось configVips.Vips, когда измененяется stand.Vips)
//Если все ок добавляем Випы из конфигуратора в стенд 
//stand.Vips = configVips.Vips;
//посе этого присваиваем каждому Випу его платку
//stand.Start();
