using Microsoft.Extensions.Logging.Abstractions;
using RJCP.IO.Ports;
using SerialPortLib;

namespace Vips
{
    public class RelaySwitch : BaseMeter
    {
        public Vip TestVip { get; set; } = new Vip();

        public void Config(string pornName, int baud, Parity parity, DataBits dataBits, StopBits stopBits)
        {
            port = new SerialPortInput(new NullLogger<SerialPortInput>());
            port.SetPort(pornName, baud, stopBits, parity, dataBits);
        }

        public void CheckedConnect(int checkedOnConnectTimes = 1)
        {
            var selectCmd = libCmd.DeviceCommands
                .FirstOrDefault(x => x.Key.NameCmd == "Status" && x.Key.NameDevice == "Relay");
            
            //Количество попыток досутчатся до прибора
            for (int i = 0; i < checkedOnConnectTimes; i++)
            {
                
                TransmitReceivedCmd(selectCmd.Value.Transmit);
                //TODO разобратся шаблон ответа должен в сбее содержать ответ прибора или наоборот
                // if (selectCmd.Value.Receive.Contains(receive))
                // {
                // }
            }

        }
    }
}