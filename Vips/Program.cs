// See https://aka.ms/new-console-template for more information

using System;
using Vips;

ConfigVips configVips = new ConfigVips();

Stand stand = new Stand();



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

//добавление приборв измерения в стенд
stand.AddDevice(TypeDevice.VoltMeter, "GDM-74303", 1, 0, 8);
stand.AddDevice(TypeDevice.Thermometer, "GDM-7433",2 , 0, 8);
stand.AddDevice(TypeDevice.Load, "GDM-4303",3 , 0, 8);


stand.AddDevice(TypeDevice.Relay, "1", 5, 0, 8);
stand.AddDevice(TypeDevice.Relay, "2", 5, 0, 8);
stand.AddDevice(TypeDevice.Relay, "3", 6, 0, 8);
stand.AddDevice(TypeDevice.Relay, "4", 7, 0, 8);
stand.AddDevice(TypeDevice.Relay, "5", 8, 0, 8);
stand.AddDevice(TypeDevice.Relay, "6", 9, 0, 8);
stand.AddDevice(TypeDevice.Relay, "7", 10, 0, 8);
stand.AddDevice(TypeDevice.Relay, "8", 11, 0, 8);
stand.AddDevice(TypeDevice.Relay, "9", 12, 0, 8);
stand.AddDevice(TypeDevice.Relay, "10", 13, 0, 8);
stand.AddDevice(TypeDevice.Relay, "11", 14, 0, 8);
stand.AddDevice(TypeDevice.Relay, "12", 15, 0, 8);


//TODO првильно ли я все написал (надо чтобы обновлялось configVips.Vips, когда измененяется stand.Vips)
//TODO может создать список выше по иерерхии или ченть такое
//TODO должо 
stand.Vips = configVips.Vips;

stand.Start();
