﻿using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using RJCP.IO.Ports;
using SerialPortLib;
using System.Threading;


namespace Vips;

public class BaseDevice
{
    /// <summary>
    /// Имя прибора
    /// </summary>
    public string Name;
    
    //TODO спросить как сделать это потокобезопасным
    /// <summary>
    /// Тип прибора
    /// </summary>
    public TypeDevice Type;

    public ConfigDeviceParams Config { get; set; } = new ConfigDeviceParams();

    /// <summary>
    /// Компорт прибора
    /// </summary>
    protected ISerialLib port;

    /// <summary>
    /// Класс библиотеки
    /// </summary>
    protected BaseLibCmd libCmd = BaseLibCmd.getInstance();

    /// <summary>
    /// Класс библиотеки
    /// </summary>
    protected TypeCmd typeReceive;


    //TODO спросить как сделать это потокобезопасным
    /// <summary>
    /// Задержка команды
    /// </summary>
    public int CmdDelay { get; set; }

    /// <summary>
    /// Событие проверки коннекта к порту
    /// </summary>
    public Action<BaseDevice, bool> ConnectPort;

    /// <summary>
    /// Событие проверки коннекта к устройству
    /// </summary>
    public Action<BaseDevice, bool> ConnectDevice;


    /// <summary>
    /// Событие приема данных с устройства
    /// </summary>
    public Action<BaseDevice, string> Receive;

    Stopwatch stopwatch = new();

    public BaseDevice(string name, TypeDevice type)
    {
        Name = name;
        Type = type;
    }

    /// <summary>
    /// Конфигурация компортра утройства
    /// </summary>
    /// <param name="typePort">Тип исопльзуемой библиотеки com port</param>
    /// <param name="pornName">Номер компорта</param>
    /// <param name="baud">Бауд рейт компорта</param>
    /// <param name="stopBits">Стоповые биты компорта</param>
    /// <param name="parity">Parity bits</param>
    /// <param name="dataBits">Data bits count</param>
    /// <param name="dtr"></param>
    /// <returns></returns>
    public void ConfigDevice()
    {
        if (Config.TypePort == TypePort.GodSerial)
        {
            port = new SerialGod();
        }

        if (Config.TypePort == TypePort.SerialInput)
        {
            port = new SerialInput();
        }

        if (port.IsOpen())
        {
            PortDisconnect();
        }

        port.SetPort(Config.PortName, Config.Baud, Config.StopBits, Config.Parity, Config.DataBits);
        port.Dtr = Config.Dtr;
        port.ConnectionStatusChanged += ConnectionStatusChanged;
        port.MessageReceived += MessageReceived;
    }

    public ConfigDeviceParams GetConfigDevice()
    {
        if (Config != null)
        {
            return Config;
        }

        throw new DeviceException("BaseDevice exception: Файл конфига отсутствует");
    }

    public void PortConnect()
    {
        port.Connect();
    }

    public void PortDisconnect()
    {
        port.Disconnect();
    }

    /// <summary>
    /// Проверка устройства на коннект
    /// </summary>
    /// <param name="checkCmd">Команда проверки не из библиотеки (если пусто будет исользована команда "Status" и прибор из библиотеки )</param>
    /// <param name="delay">Задержка на проверку (если 0 будет исользована из библиотеки )</param>
    /// <returns>Успешна ли попытка коннекта</returns>
    /// <exception cref="DeviceException">Такого устройства, нет в библиотеке команд</exception>
    public void CheckedConnectDevice(string checkCmd = "", int delay = 0, string terminator = "")
    {
        //для отладки
        // Время начала 
        if (!stopwatch.IsRunning)
        {
            stopwatch.Start();
        }

        //

        //если строка команды пустая
        if (string.IsNullOrWhiteSpace(checkCmd))
        {
            //используем команду статус которя возмет текущий прибор и введет в него команду статус
            TransmitCmdInLib("Status");
        }
        else
        {
            //используем ручной ввод
            port.TransmitCmdTextString(cmd: checkCmd, delay: delay, terminator: terminator);
        }
    }

    /// <summary>
    /// Отправка в устройство и прием СТАНДАРТНЫХ (есть в библиотеке команд) команд из устройства
    /// </summary>
    /// <param name="nameCommand">Имя команды (например Status)</param>
    public void TransmitCmdInLib(string nameCommand)
    {
        // MeterCmd selectCmd = libCmd.DeviceCommands
        //     .FirstOrDefault(x => x.Key.NameCmd == nameCommand && x.Key.NameDevice == Name).Value;
        var selectCmd = GetLibItem(nameCommand, Name);

        if (selectCmd == null)
        {
            throw new DeviceException(
                $"BaseDevice exception: Такое устройство - {Name} или команда - {nameCommand} в библиотеке не найдены");
        }

        CmdDelay = selectCmd.Delay;

        if (selectCmd.MessageType == TypeCmd.Hex)
        {
            typeReceive = TypeCmd.Hex;
            port.TransmitCmdHexString(selectCmd.Transmit, selectCmd.Delay,
                selectCmd.StartOfString, selectCmd.EndOfString,
                selectCmd.Terminator);
        }
        else
        {
            typeReceive = TypeCmd.Text;
            port.TransmitCmdTextString(selectCmd.Transmit, selectCmd.Delay, selectCmd.StartOfString,
                selectCmd.EndOfString,
                selectCmd.Terminator);
        }
    }

    /// <summary>
    /// Выббор команды из библиотеке осноываясь на ее имени и имени прибора
    /// </summary>
    /// <param name="cmd">Имя команды</param>
    /// <param name="deviceName">Имя прибора</param>
    /// <returns></returns>
    protected DeviceCmd GetLibItem(string cmd, string deviceName)
    {
        return libCmd.DeviceCommands
            .FirstOrDefault(x => x.Key.NameCmd == cmd && x.Key.NameDevice == deviceName).Value;
    }

    /// <summary>
    /// Прошел ли коннект выбраного com port
    /// </summary>
    private void ConnectionStatusChanged(bool isConnect)
    {
        //для отладки
        //Console.WriteLine($"BaseDevice message: Время работы программы: {stopwatch.Elapsed.TotalMilliseconds} милисекунд");
        //stopwatch.Restart(); // Остановить отсчет времени
        //для отладки
        //Console.WriteLine($"BaseDevice message: Oтвет от порта {port.GetPortNum} - {isConnect}, устройство {Name}");
        ConnectPort.Invoke(this, true);
    }

    /// <summary>
    /// Обработка прнятого сообщения из устройства
    /// </summary>
    private void MessageReceived(string receive)
    {
        //для отладки
        // Console.WriteLine(
        //     $"BaseDevice message: Время работы программы: {stopwatch.Elapsed.TotalMilliseconds} милисекунд");

        //для отладки
        //Console.WriteLine($"BaseDevice message: ответ от устройства {Name} - {receive}, порт {port.GetPortNum} ");
        //
        //для проверки на статус 
        var selectCmd = GetLibItem("Status", Name);
        //если ответ от устройства соотвествует ответу на кодмаду Status то вернем true


        if (typeReceive == TypeCmd.Text)
        {
            
            if (receive.Contains(selectCmd.Receive))
            {
                ConnectDevice.Invoke(this, true);
            }
            Receive.Invoke(this, receive);
        }
        if (typeReceive == TypeCmd.Hex)
        {  
            if (GetStringTextInHex(receive).Contains(selectCmd.Receive))
            {
                ConnectDevice.Invoke(this, true);
            }
            Receive.Invoke(this, GetStringTextInHex(receive));
        }

    }

    string GetStringTextInHex(string s)
    {
        if (!string.IsNullOrEmpty(s))
        {
            byte[] bytes = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                var ff = bytes[i / 2];
                bytes[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);
            }

            return Encoding.ASCII.GetString(bytes);
        }

        return "";
    }
}