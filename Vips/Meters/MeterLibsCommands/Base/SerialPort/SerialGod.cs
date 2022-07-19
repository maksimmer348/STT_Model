using System.Text;
using GodSharp.SerialPort;
using RJCP.IO.Ports;
using SerialPortLib;

namespace Vips;

public class SerialGod : ISerialLib
{
    private GodSerialPort port;
    public bool Dtr { get; set; }
    public string GetPortNum { get; set; }

    public int Delay { get; set; }
    public Action<bool> ConnectionStatusChanged { get; set; }
    public Action<string> MessageReceived { get; set; }


    public void SetPort(string pornName, int baud, int stopBits, int parity, int dataBits, bool dtr = false)
    {
        try
        {
            port = new GodSerialPort(pornName, baud, parity, dataBits, stopBits);
            port.DtrEnable = true;
            GetPortNum = pornName;
        }
        catch (DeviceException e)
        {
            throw new DeviceException($"Порт \"{GetPortNum}\" не конфигурирован, ошибка - {e.Message}");
        }
    }

    public bool Connect()
    {
        try
        {
            var isConnect = port.Open();
            ConnectionStatusChanged?.Invoke(isConnect);

            return isConnect;
        }
        catch (DeviceException e)
        {
            throw new DeviceException($"Порт \"{GetPortNum}\" не отвечает, ошибка - {e.Message}");
        }
    }

    public void Disconnect()
    {
        try
        {
            port.Close();
        }
        catch (Exception e)
        {
            throw new DeviceException($"Порт \"{GetPortNum}\" не отвечает, ошибка - {e.Message}");
        }
    }


    // "0A""0D"
    private string[] trashStr = {"FE", "FC", "F8", "FF", "F0", "E0", " ", "C0"};

    public string ReadPSP(string startOfString, string endOfString = "", int innerCount = 10, int countReads = 3)
    {
        int innerNullCount = innerCount;
        int innerErrorCount = innerCount;

        for (int i = 0; i < countReads; i++)
        {
            var s = port.ReadString();
            //если не прочиатлась строка 
            while (s == null)
            {
                //пытаемся еще раз прочиать
                s = port.ReadString();
                Thread.Sleep(10);
                //если innerCount раз не прочиатлась то вызов исключения
                innerNullCount--;

                if (innerNullCount == 0)
                {
                    throw new Exception($"Ответа нет (null), удачных попыток {innerCount}");
                }
            }

            //удаляем мусори и пробелы из строки
            foreach (var str in trashStr)
            {
                s = s.Replace(str, "");
            }

            //если строка содержит входной символ
            //TODO переделать (57 символ отправленный устройтву в хекс)
            while (s.Contains(startOfString))
            {
                //то мы прибавляем строку из буффера компорта
                s += port.ReadString();
                s = s.Replace(" ", "");
                //TODO переделать (0D == \r, 0A == \n)
                //проверяемем есть ли в строке сиволы окончания
                if (string.Equals(s.Substring(s.Length - endOfString.Length), endOfString, StringComparison.CurrentCultureIgnoreCase))
                {
                    //проверка на дублирование ответа если дублирован убираем 2 половину
                    if (s.Substring(0, s.Length / 2) == s.Substring(s.Length / 2, s.Length / 2))
                    {
                        s = s.Substring(0, s.Length / 2);
                    }

                    //возвращаем строку
                    MessageReceived.Invoke(s);
                    return s;
                }

                innerErrorCount--;
                //тк мы в while если количевто попыток прочиать строку превысит 10 попыток то выходим из цикла
                //и выбрасываем исключение
                if (innerErrorCount <= 0)
                {
                    throw new Exception($"Слишком много неудачых попыток (notNull) - {innerCount}");
                }

                Thread.Sleep(10);
            }

            if (!s.Contains(startOfString))
            {
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
                Thread.Sleep(20);
                TransmitCmd(startOfString);
            }
        }

        throw new Exception($"Ответа нет, неудачых попыток {countReads}");
    }

    public void TransmitCmd(string cmd, int delay = 0, string start = "", string end = "", string terminator = "")
    {
        if (string.IsNullOrEmpty(cmd))
        {
            throw new DeviceException($"При ручном вводе, checkCmd- не должны быть пустыми");
        }
        
        if (string.IsNullOrEmpty(terminator))
        {
            terminator = "0A0D";
        }

        Delay = delay;
        try
        {
            port.WriteHexString(cmd + terminator);
            Thread.Sleep(30);
            ReadPSP(start, end);
        }
        catch (DeviceException e)
        {
            throw new DeviceException(e.Message);
        }
    }
}