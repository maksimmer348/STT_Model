using Microsoft.Extensions.Logging.Abstractions;
using RJCP.IO.Ports;
using SerialPortLib;

namespace Vips;

public class SerialInput : ISerialLib
{
    protected SerialPortInput port;
    public bool Dtr { get; set; }
    public string GetPortNum { get; set; }
    public int Delay { get; set; }
    public Action<bool> ConnectionStatusChanged { get; set; }
    public Action<string> MessageReceived { get; set; }

    public void SetPort(string pornName, int baud, int stopBits, int parity, int dataBits, bool dtr = false)
    {
        var adaptSettings = SetPortAdapter(stopBits, parity, dataBits);
        port = new SerialPortInput(new NullLogger<SerialPortInput>());
        try
        {
            port.SetPort(pornName, baud, adaptSettings.Item1, adaptSettings.Item2, adaptSettings.Item3);
        }
        catch (DeviceException e)
        {
            throw new DeviceException($"Порт \"{GetPortNum}\" не конфигурирован, ошибка - {e.Message}");
        }

        port.ConnectionStatusChanged += OnPortConnectionStatusChanged;
        port.MessageReceived += OnPortMessageReceived;
        GetPortNum = pornName;
    }


    /// <summary>
    /// Прием сообщения из устройства
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args">Сообщение от устройства</param>
    public void OnPortMessageReceived(object sender, MessageReceivedEventArgs args)
    {
        var data = System.Text.Encoding.UTF8.GetString(args.Data);
        var answer = (data);
        MessageReceived.Invoke(answer);
    }

    /// <summary>
    /// Прием ответа соединения от serial port
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args">Наличие коннекта true/false</param>
    public void OnPortConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs args)
    {
        bool conn = args.Connected;
        ConnectionStatusChanged.Invoke(conn);
        port.DtrEnable = Dtr;
    }

    public bool Connect()
    {
        var isConnect = port.Connect();
        if (isConnect)
        {
            Thread.Sleep(20);
            return isConnect;
        }
        
        throw new DeviceException($"Порт \"{GetPortNum}\" не отвечает");
    }

    public void Disconnect()
    {
        try
        {
            port.Disconnect();
        }
        catch (Exception e)
        {
            throw new DeviceException($"Порт \"{GetPortNum}\" не отвечает, ошибка - {e.Message}");
        }
    }


    /// <summary>
    /// Адаптер значений для библиотеки 
    /// </summary>
    /// <param name="sBits">Stop bits (1-2)</param>
    /// <param name="par">Patyty bits (0-2)</param>
    /// <param name="dBits">Data bits (5-8)</param>
    /// <returns></returns>
    public (StopBits, Parity, DataBits) SetPortAdapter(int sBits, int par, int dBits)
    {
        StopBits stopBits = StopBits.One;
        Parity parity = Parity.None;
        DataBits dataBits = DataBits.Eight;

        stopBits = sBits switch
        {
            1 => StopBits.One,
            2 => StopBits.Two,
            _ => stopBits
        };

        parity = par switch
        {
            0 => Parity.None,
            1 => Parity.Odd,
            2 => Parity.Even,
            _ => parity
        };
        dataBits = dBits switch
        {
            5 => DataBits.Five,
            6 => DataBits.Six,
            7 => DataBits.Seven,
            8 => DataBits.Eight,
            _ => dataBits
        };
        return (stopBits, parity, dataBits);
    }

    /// <summary>
    /// Отправка в устройство и прием команд из устройства
    /// </summary>
    /// <param name="cmd">Команда</param>
    /// <param name="delay">Задержка между запросом и ответом</param>
    /// <param name="receiveType"></param>
    /// <param name="terminator">Окончание строки команды по умолчанию \r\n</param>
    /// <returns>Ответ от устройства</returns>
    public void TransmitCmd(string cmd, int delay = 0, string start = "", string end = "", string terminator = "")
    {
        if (string.IsNullOrEmpty(cmd))
        {
            throw new DeviceException($"Команда - не должны быть пустыми");
        }
        if (delay == 0)
        {
            throw new DeviceException($"Задержка - не должны быть = 0");
        }
        if (string.IsNullOrEmpty(terminator))
        {
            terminator = "\r\n";
        }

        Delay = delay;
        var message = System.Text.Encoding.UTF8.GetBytes(cmd + terminator);
        port.SendMessage(message);
    }
}